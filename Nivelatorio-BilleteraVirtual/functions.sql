CREATE OR REPLACE FUNCTION wallet.registrar_transaccion(
    p_numero_cuenta VARCHAR(20),  
    p_nombre_categoria VARCHAR(50), 
    p_monto DECIMAL(18,2),
    p_tipo VARCHAR(10)
)
    RETURNS TABLE (
                      Id INT,
                      PublicId UUID,
                      CuentaId INT,
                      CategoriaId INT,
                      Monto DECIMAL(18,2),
                      Tipo VARCHAR(10),
                      Fecha TIMESTAMP
                  ) AS $$
DECLARE
    v_cuenta_id INT;             
    v_categoria_id INT;          
    v_saldo_actual DECIMAL(18,2);
    v_cat_tipo VARCHAR(10);
    v_transaccion_id INT;
BEGIN
    -- Recordar usar minúsculas para las columnas de Postgres si usaste nombres mixtos sin comillas

    -- 1. Validar existencia de Cuenta mediante el Numero de Cuenta y bloquear fila
    SELECT c.id, c.saldo INTO v_cuenta_id, v_saldo_actual
    FROM wallet.cuentas c
    WHERE c.numerocuenta = p_numero_cuenta FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'La cuenta con número % no existe', p_numero_cuenta;
    END IF;

    -- 2. Validar existencia de Categoría mediante el Nombre y obtener su Id y Tipo
    SELECT cat.id, cat.tipo INTO v_categoria_id, v_cat_tipo
    FROM wallet.categorias cat
    WHERE cat.nombre = p_nombre_categoria;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'La categoría «%» no existe', p_nombre_categoria;
    END IF;

    -- 3. Valida consistencia de tipos
    IF v_cat_tipo <> p_tipo THEN
        RAISE EXCEPTION 'Inconsistencia de tipos: La categoría es de tipo % pero la transacción se envió como %', v_cat_tipo, p_tipo;
    END IF;

    -- 4. Si es Egreso: Verifica saldo suficiente e impacta la cuenta usando el ID hallado
    IF p_tipo = 'EGRESO' THEN
        -- ... validación de saldo ...
        UPDATE wallet.cuentas SET saldo = saldo - p_monto WHERE wallet.cuentas.id = v_cuenta_id;
    ELSE
        UPDATE wallet.cuentas SET saldo = saldo + p_monto WHERE wallet.cuentas.id = v_cuenta_id;
    END IF;

    -- 5. Registrar la transacción usando las IDs internas que acabamos de averiguar
    INSERT INTO wallet.transacciones AS t (cuentaid, categoriaid, monto, tipo, fecha)
    VALUES (v_cuenta_id, v_categoria_id, p_monto, p_tipo, CURRENT_TIMESTAMP)
    RETURNING t.id INTO v_transaccion_id;
    
    -- 6. Devolver el registro completo creado
    RETURN QUERY
        SELECT t.id, t.publicid, t.cuentaid, t.categoriaid, t.monto, t.tipo, t.fecha
        FROM wallet.transacciones t
        WHERE t.id = v_transaccion_id;

END;
$$ LANGUAGE plpgsql;