using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Registry;
public static class RegistryOperations
{
    /// <summary>
    /// Retrieves the value. If the value is null, throws an exception.
    /// </summary>
    /// <param name="registryItem"></param>
    /// <returns></returns>
    /// <exception cref="InvalidRegistryItemException">Registry item is invalid or does not have valid valueName.</exception>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    public static object GetValue(RegistryItem registryItem)
    {
        if (registryItem.IsValid() != string.Empty)
            throw new InvalidRegistryItemException(registryItem, registryItem.IsValid());

        using RegistryKey? key = registryItem.GetRegistryKey();

        object o = key?.GetValue(registryItem.ValueName) ?? throw new InvalidOperationException();

        return o;

    }

    /// <summary>
    ///   Retrieves the value. If the value is null, returns null.
    /// </summary>
    /// <param name="registryItem"></param>
    /// <returns></returns>
    /// <exception cref="InvalidRegistryItemException"></exception>
    public static object? GetValueNullable(RegistryItem? registryItem)
    {
        if (registryItem is null)
            return null;
        //If value is null and could not be created.
        if (registryItem.ValueName is null)
            return null;

        if (registryItem.IsValid() != string.Empty)
            throw new InvalidRegistryItemException(registryItem, registryItem.IsValid());

        using RegistryKey? key = registryItem.GetRegistryKey();

        return key?.GetValue(registryItem.ValueName);
    }

    /// <summary>
    /// Add a new registry or edit if exists. Method checks, if value of registry is equal the parameter value, does not update registry.
    /// </summary>
    /// <param name="registryItem"> The registryItem </param>
    /// <param name="value">The value for inserting as registry key value.</param>
    /// <exception cref="ArgumentNullException">subKey or keyName is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="ArgumentException">Registry not found or key is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">The user does not have the necessary registry rights.</exception>
    /// <exception cref="System.Security.SecurityException">The user does not have the permissions required to read from the registry key.</exception>
    public static void EditAddRegistryValue(RegistryItem? registryItem, object value)
    {

        if (!string.IsNullOrWhiteSpace(registryItem.IsValid()))
            throw new ArgumentNullException(nameof(registryItem), registryItem.IsValid());

        if (value == null)
            throw new ArgumentNullException(nameof(value), "The registry value could not be null. (EditAddRegistryValue)");

        object? existsRegistry = GetValueNullable(registryItem);

        // If registry value is equal new value -> just do nothing :-)
        if (existsRegistry != null && existsRegistry.ToString() == value.ToString())
        {
            Debug.WriteLine($"\t-Registry value (old): {existsRegistry} = Registry value (new): {value.ToString()}");
            return;
        }
        registryItem.SetValue(value);

    }

    /// <summary>
    /// Delete a registry or subkey of registry if exists.
    /// If ValueName of the registry item is <see langword="null"/>, function deletes the specified <b>Subkey</b>.
    /// If ValueName of the registry item is not <see langword="null"/>, function deletes the specified <b>ValueName</b>.
    /// </summary>
    /// <param name="registryItem"></param>
    /// <exception cref="ArgumentNullException">subKey is null.</exception>
    /// <exception cref="ObjectDisposedException">The RegistryKey is closed (closed keys cannot be accessed).</exception>
    /// <exception cref="SecurityException">The user does not have the permissions required to delete the value.</exception>
    /// <exception cref="UnauthorizedAccessException">The RegistryKey being manipulated is read-only.</exception>
    public static void DeleteRegistryValueNameOrSubKey(RegistryItem registryItem)
    {

        if (registryItem.IsValid() == string.Empty)
            return;
        //    throw new NullReferenceException("Registry subkey does not exist.");
        if (registryItem.ValueName == null)
        {
            //Delete subkey of registry if exists.
            registryItem.DeleteSubKey();
        }
        else
        {
            //deletes the specified ValueName.
            registryItem.DeleteValueName();
        }

    }
    /// <summary>
    /// Recursively get all registry value and names
    ///<para>
    /// RETURNS NO EXCEPTION!
    /// </para>
    /// </summary>
    /// <returns></returns>
    /// 
    public static string GetAllRegistriesRecursiveDefaultAuxalia()
    {
        using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
            .OpenSubKey("SOFTWARE\\auxalia");
        return OutputRegKey(registryKey).Replace("HKEY_CURRENT_USER\\SOFTWARE\\", "");
    }

    /// <summary>
    /// Recursively get all registry value and names
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    private static string OutputRegKey(RegistryKey Key)
    {
        try
        {
            string result = "";
            string[] subkeynames = Key.GetSubKeyNames(); //means deeper folder

            if (subkeynames == null || subkeynames.Length <= 0)
            { //has no more subkey, process
                result += ProcessValueNames(Key);

            }
            else
            {
                result += ProcessValueNames(Key);
                foreach (string keyname in subkeynames)
                { //has subkeys, go deeper
                    using (RegistryKey key2 = Key.OpenSubKey(keyname))
                        result += OutputRegKey(key2);
                }
            }

            return result;
        }
        catch (Exception e)
        {
            //error, do something
            Console.WriteLine("Error OutputRegKey. " + e.Message);
            return "Error getting GetAllRegistriesRecursiveDefaultAuxalia!";
            //throw;
        }
    }

    /// <summary>
    /// Gets the value names for a given key
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    private static string ProcessValueNames(RegistryKey Key)
    { //function to process the valueNames for a given key
        string result = "";
        string[] valuenames = Key.GetValueNames();
        if (valuenames == null || valuenames.Length <= 0) //has no values
            return result;
        foreach (string valuename in valuenames)
        {
            object obj = Key.GetValue(valuename);
            if (obj != null)
                result += Key.Name + "\\ " + valuename + "\t" + obj.ToString() + "\n";
        }

        return result;
    }

}
