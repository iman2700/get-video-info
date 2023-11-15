using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Registry;
public class RegistryItem : IEquatable<RegistryItem>
{
    public const string Root = "HKEY_CURRENT_USER";
    /// <summary>
    /// The <b>HKEY_CURRENT_USER</b> base regirty key.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">
    /// If the regitry key of the "HKEY_CURRENT_USER" cannot be retrieved. 
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">
    /// The user does not have the necessary registry rights.
    /// </exception>
    /// <exception cref="System.Security.SecurityException">
    /// The user does not have the permissions required to perform this action.
    /// </exception>
    private static RegistryKey CurrentUserBaseKey =>
        // Opens the "Current User" registry key
        RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);

    /// <summary>
    /// The <b>HKEY_CURRENT_USER</b> base regirty key.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">
    /// If the regitry key of the "HKEY_CURRENT_USER" cannot be retrieved. 
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">
    /// The user does not have the necessary registry rights.
    /// </exception>
    /// <exception cref="System.Security.SecurityException">
    /// The user does not have the permissions required to perform this action.
    /// </exception>
    private static RegistryKey LocalMachineBaseKey =>
        // Opens the "Current User" registry key
        RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
    public string SubKey { get; }
    public string? ValueName { get; }

    /// <summary>
    /// Creates RegistryItem object.
    /// </summary>
    /// <param name="subKeyStrings">As an array of strings ["ProjectBox","2022"] or ["ProjectBox\\2022"] or ["Software\\auxalia\\ProjectBox\\2022"]</param>
    /// <param name="valueName"></param>
    public RegistryItem(string[] subKeyStrings, string? valueName)
    {
        SubKey = CreateSubKey(subKeyStrings);
        ValueName = valueName;
    }

    /// <summary>
    /// Creates RegistryItem object.
    /// </summary>
    /// <param name="subKeyStrings">As an array of strings ["ProjectBox","2022"] or ["ProjectBox\\2022"] or ["Software\\auxalia\\ProjectBox\\2022"]</param>
    public RegistryItem(string[] subKeyStrings)
    {
        SubKey = CreateSubKey(subKeyStrings);
    }

    public string CreateSubKey(string[] subKeyStrings, string subKeyA = "SOFTWARE", string subKeyB = "auxalia")
    {
        if (subKeyStrings[0].ToLower().StartsWith("software\\auxalia"))
            return $"{string.Join("\\", subKeyStrings)}";

        return $"{subKeyA}\\{subKeyB}\\{string.Join("\\", subKeyStrings)}";
    }

    /// <summary>
    /// It gets Empty string if registry is valid. Otherwise, it returns the error message.
    /// </summary>
    /// <returns></returns>
    public string IsValid()
    {
        //if (!SubKey.Contains("HKEY_CURRENT_USER"))
        //    return $"The subKey{SubKey} should be in HKEY_CURRENT_USER key.";
        if (!SubKey.Contains("auxalia"))
            return $"The subKey{SubKey} should be in auxalia subKey.";

        if (string.IsNullOrEmpty(ValueName))
            return $"The value could not be null or empty.";
        //if (GetRegistryKey() is null) //NULL = Registry Does not exist
        //    return "Could not find the registryKey.";
        return string.Empty;
    }

    /// <summary>
    /// Gets the registryKey object of specified subKey of the object.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="SecurityException">The user does not have the permissions required to access the registry key in the specified mode.</exception>
    public RegistryKey? GetRegistryKey()
    {
        return CurrentUserBaseKey.OpenSubKey(SubKey);
    }

    /// <summary>
    /// Set the value of the registry item.
    /// </summary>
    /// <exception cref="Exception">If the registry is not valid.</exception>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="SecurityException">The user does not have the permissions required to access the registry key in the specified mode.</exception>">
    public void SetValue(object value)
    {
        if (IsValid() != string.Empty)
            throw new Exception(IsValid());

        RegistryKey? registryKey = CurrentUserBaseKey.OpenSubKey(SubKey, true);
        if (registryKey is not null)
            registryKey.SetValue(ValueName, value);
        else
            throw new Exception($"Could not find the registryKey of {SubKey}.");
    }

    /// <summary>
    /// This method deletes the subkey of the registry item.
    /// </summary>
    /// <exception cref="Exception">If the registry is not valid.</exception>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="SecurityException">The user does not have the permissions required to access the registry key in the specified mode.</exception>">
    /// <exception cref="UnauthorizedAccessException">The user does not have the necessary registry rights.</exception>
    /// <exception cref="ArgumentException">The subkey cannot be deleted.</exception>
    public void DeleteSubKey()
    {
        if (IsValid() != string.Empty)
            throw new Exception(IsValid());

        RegistryKey? registryKey = CurrentUserBaseKey.OpenSubKey(SubKey, true);
        if (registryKey is not null)
            CurrentUserBaseKey.DeleteSubKey(SubKey);
        else
            throw new Exception($"Could not find the registryKey of {SubKey}.");
    }

    /// <summary>
    /// This method deletes the valueName of the registry item.
    /// </summary>
    /// <exception cref="Exception">If the registry is not valid.</exception>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="SecurityException">The user does not have the permissions required to access the registry key in the specified mode.</exception>">
    /// <exception cref="UnauthorizedAccessException">The user does not have the necessary registry rights.</exception>
    /// <exception cref="ArgumentException">The subkey cannot be deleted.</exception>
    public void DeleteValueName()
    {
        if (IsValid() != string.Empty)
            throw new Exception(IsValid());

        RegistryKey? registryKey = CurrentUserBaseKey.OpenSubKey(SubKey, true);
        if (registryKey is not null)
        {
            if (ValueName != null)
                registryKey.DeleteValue(ValueName);
        }
        else
            throw new Exception($"Could not find the registryKey of {SubKey}.");
    }

    public bool Equals(RegistryItem? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return SubKey == other.SubKey && ValueName == other.ValueName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((RegistryItem)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (SubKey.GetHashCode() * 397) ^ (ValueName != null ? ValueName.GetHashCode() : 0);
        }
    }
}
