﻿using System.Text.Json;
using Microsoft.JSInterop;

namespace Dashboard.Services;

public interface ILocalStorageService
{
    Task<T> GetItem<T>(string key);
    Task SetItem<T>(string key, T value);
    Task RemoveItem(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T> GetItem<T>(string key)
    {
        string? json = null;
        try
        {
            json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

        if (json == null)
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItem<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItem(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}