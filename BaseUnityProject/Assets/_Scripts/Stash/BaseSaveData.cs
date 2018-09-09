using UnityEngine;
using System.Collections.Generic;
using System;

public class BaseSaveData {

    /// <summary>
    /// Loads data from the given string.
    /// </summary>
    public virtual void loadFromString(string str) {
        throw new NotImplementedException("Must override BaseSaveData.loadFromString().");
    }

    /// <summary>
    /// Saves data to the given string.
    /// </summary>
    public virtual string saveToString() {
        throw new NotImplementedException("Must override BaseSaveData.saveToString().");
    }

    /// <summary>
    /// Clears the save data and fills it with default values.
    /// </summary>
    public virtual void clearDefault() {
        throw new NotImplementedException("Must override BaseSaveData.clearDefault().");
    }
    
}


