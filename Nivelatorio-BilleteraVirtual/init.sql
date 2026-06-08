CREATE TABLE wallet.cuentas (
                         Id SERIAL PRIMARY KEY,
                         NombreTitular VARCHAR(100) NOT NULL,
                         NumeroCuenta VARCHAR(20) NOT NULL UNIQUE,
                         Saldo DECIMAL(18,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE wallet.categorias (
                            Id SERIAL PRIMARY KEY,
                            Nombre VARCHAR(50) NOT NULL,
                            Tipo VARCHAR(10) NOT NULL CHECK (Tipo IN ('INGRESO', 'EGRESO'))
);

CREATE TABLE wallet.transacciones (
                               Id SERIAL PRIMARY KEY,
                               CuentaId INT NOT NULL,
                               CategoriaId INT NOT NULL,
                               Monto DECIMAL(18,2) NOT NULL,
                               Tipo VARCHAR(10) NOT NULL CHECK (Tipo IN ('INGRESO', 'EGRESO')),
                               Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Restricciones de Llaves Foráneas (Foreign Keys)
                               CONSTRAINT FK_Transacciones_Cuentas FOREIGN KEY (CuentaId) REFERENCES wallet.cuentas(Id),
                               CONSTRAINT FK_Transacciones_Categorias FOREIGN KEY (CategoriaId) REFERENCES wallet.categorias(Id)
);

