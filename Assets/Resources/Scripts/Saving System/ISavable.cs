using System;

public interface ISavable
{
    int GetPriority();
    void LoadData(GameData data);
    void SaveData(GameData data);
}
