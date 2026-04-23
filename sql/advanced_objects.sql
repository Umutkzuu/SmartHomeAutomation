USE SmartHomeDB;

DELIMITER //

CREATE TRIGGER trg_AfterDeviceUpdate
AFTER UPDATE ON DEVICES
FOR EACH ROW
BEGIN
    
    IF (OLD.IsActive != NEW.IsActive) OR (OLD.CurrentValue != NEW.CurrentValue) THEN
        INSERT INTO DEVICE_LOGS (DeviceID, ActionType, OldValue, NewValue)
        VALUES (
            NEW.DeviceID,
            CASE 
                WHEN OLD.IsActive = 0 AND NEW.IsActive = 1 THEN 'Açıldı'
                WHEN OLD.IsActive = 1 AND NEW.IsActive = 0 THEN 'Kapandı'
                ELSE 'Değer Değişti'
            END,
            OLD.CurrentValue,
            NEW.CurrentValue
        );
    END IF;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE sp_TurnOffRoomDevices(IN p_RoomID INT)
BEGIN
    
    UPDATE DEVICES
    SET IsActive = FALSE, CurrentValue = 0
    WHERE RoomID = p_RoomID AND IsActive = TRUE;
END //

DELIMITER ;

CREATE OR REPLACE VIEW vw_ActiveDevicesDashboard AS
SELECT 
    d.DeviceID,
    d.DeviceName,
    r.RoomName,
    c.CategoryName,
    d.CurrentValue,
    c.UnitSymbol
FROM DEVICES d
JOIN ROOMS r ON d.RoomID = r.RoomID
JOIN DEVICE_CATEGORIES c ON d.CategoryID = c.CategoryID
WHERE d.IsActive = TRUE;




CREATE INDEX idx_DeviceName ON DEVICES(DeviceName);


CREATE INDEX idx_LogTimestamp ON DEVICE_LOGS(Timestamp);

DELIMITER //

CREATE PROCEDURE sp_TurnOnRoomDevices(IN p_RoomID INT)
BEGIN
    UPDATE DEVICES
    SET IsActive = TRUE
    WHERE RoomID = p_RoomID AND IsOnline = TRUE;
END //

DELIMITER ;