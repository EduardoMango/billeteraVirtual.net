CREATE TABLE wallet.cuentas (
                         Id SERIAL PRIMARY KEY,
                         NombreTitular VARCHAR(100) NOT NULL,
                         NumeroCuenta VARCHAR(20) NOT NULL UNIQUE,
                         Saldo DECIMAL(18,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE wallet.categorias (
                            Id SERIAL PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Type VARCHAR(10) NOT NULL CHECK (Type IN ('INGRESO', 'EGRESO'))
);

CREATE TABLE wallet.transacciones (
                               Id SERIAL PRIMARY KEY,
                               publicid UUID NOT NULL DEFAULT gen_random_uuid(),
                               CuentaId INT NOT NULL,
                               CategoriaId INT NOT NULL,
                               Monto DECIMAL(18,2) NOT NULL,
                               Tipo VARCHAR(10) NOT NULL CHECK (Tipo IN ('INGRESO', 'EGRESO')),
                               Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Restricciones de Llaves Foráneas (Foreign Keys)
                               CONSTRAINT FK_Transacciones_Cuentas FOREIGN KEY (CuentaId) REFERENCES wallet.cuentas(Id),
                               CONSTRAINT FK_Transacciones_Categorias FOREIGN KEY (CategoriaId) REFERENCES wallet.categorias(Id)
);

CREATE UNIQUE INDEX idx_transacciones_publicid ON wallet.transacciones(publicid);
