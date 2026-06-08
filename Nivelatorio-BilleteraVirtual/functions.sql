CREATE OR REPLACE FUNCTION wallet.registrar_transaccion(
    p_cuenta_id INT,
    p_categoria_id INT,
    p_monto DECIMAL(18,2),
    p_tipo VARCHAR(10)
)
    RETURNS TABLE (
                      Id INT,
                      CuentaId INT,
                      CategoriaId INT,
                      Monto DECIMAL(18,2),
                      Tipo VARCHAR(10),
                      Fecha TIMESTAMP
                  ) AS $$
DECLARE
    v_saldo_actual DECIMAL(18,2);
    v_cat_tipo VARCHAR(10);
    v_transaccion_id INT;
BEGIN
    -- 1. Validar existencia de Cuenta y obtener saldo actual con bloqueo (FOR UPDATE)
    -- Esto evita que otra transacción modifique el saldo mientras procesamos esta
    SELECT Saldo INTO v_saldo_actual
    FROM wallet.cuentas
    WHERE Id = p_cuenta_id FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'La cuenta con ID % no existe', p_cuenta_id;
    END IF;

    -- 2. Validar existencia de Categoría y obtener su Tipo
    SELECT Tipo INTO v_cat_tipo
    FROM wallet.categorias
    WHERE Id = p_categoria_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'La categoría con ID % no existe', p_categoria_id;
    END IF;

    -- 3. Valida consistencia de tipos (Que el tipo enviado coincida con la categoría)
    IF v_cat_tipo <> p_tipo THEN
        RAISE EXCEPTION 'Inconsistencia de tipos: La categoría es de tipo % pero la transacción se envió como %', v_cat_tipo, p_tipo;
    END IF;

    -- 4. Si es Egreso: Verifica saldo suficiente e impacta la cuenta
    IF p_tipo = 'EGRESO' THEN
        IF v_saldo_actual < p_monto THEN
            RAISE EXCEPTION 'Saldo insuficiente. Saldo actual: %, Monto requerido: %', v_saldo_actual, p_monto;
        END IF;

        -- Restar saldo
        UPDATE wallet.cuentas SET Saldo = Saldo - p_monto WHERE Id = p_cuenta_id;
    ELSE
        -- Si es Ingreso, sumar saldo
        UPDATE wallet.cuentas SET Saldo = Saldo + p_monto WHERE Id = p_cuenta_id;
    END IF;

    -- 5. Registrar la transacción en el historial
    INSERT INTO wallet.transacciones (CuentaId, CategoriaId, Monto, Tipo, Fecha)
    VALUES (p_cuenta_id, p_categoria_id, p_monto, p_tipo, CURRENT_TIMESTAMP)
    RETURNING wallet.transacciones.Id INTO v_transaccion_id;

    -- 6. Devolver el registro completo creado
    RETURN QUERY
        SELECT t.Id, t.CuentaId, t.CategoriaId, t.Monto, t.Tipo, t.Fecha
        FROM wallet.transacciones t
        WHERE t.Id = v_transaccion_id;

END;
$$ LANGUAGE plpgsql;