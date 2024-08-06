﻿using CT_MKWII_WPF.Services.Settings;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class GameData
{
    public List<User> Users { get; set; }

    public GameData()
    {
        Users = new List<User>(4);
    }

}
