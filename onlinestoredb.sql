CREATE TABLE `users` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Username` VARCHAR(50) NOT NULL,
    `PasswordHash` VARCHAR(255) NOT NULL,  -- Хранение хеша пароля
    `Email` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `Username_UNIQUE` (`Username`)
) ENGINE=InnoDB;

CREATE TABLE `products` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `Description` TEXT,
    `Price` DECIMAL(10, 2) NOT NULL,
    `Stock` INT NOT NULL,
    `Currency` VARCHAR(10) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB;

CREATE TABLE `carts` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` INT NOT NULL,
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB;


CREATE TABLE `cart_items` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ProductId` INT NOT NULL,
    `Quantity` INT NOT NULL,
    `CartId` INT NOT NULL,
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`ProductId`) REFERENCES `products`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CartId`) REFERENCES `carts`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB;
