using System;
using System.Globalization;
using UnityEngine;

public static class DateSelection
{
    public static DateTime? Pickup { get; private set; }
    public static DateTime? Drop { get; private set; }
    public static event Action Changed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnLoad()
    {
        Pickup = null;
        Drop = null;
        Changed = null;
    }

    public static void SetPickup(DateTime d)
    {
        Pickup = d.Date;
        if (Drop.HasValue && Drop.Value <= Pickup.Value) Drop = null;
        Changed?.Invoke();
    }

    public static void SetDrop(DateTime d)
    {
        Drop = d.Date;
        Changed?.Invoke();
    }

    public static void Clear()
    {
        Pickup = null;
        Drop = null;
        Changed?.Invoke();
    }

    public static string Short(DateTime d)
    {
        return d.ToString("ddd d MMM", CultureInfo.InvariantCulture);   // Fri 9 Oct
    }

    public static string Long(DateTime d)
    {
        return d.ToString("dddd d MMM", CultureInfo.InvariantCulture);  // Friday 9 Oct
    }

    public static string FromToText()
    {
        if (!Pickup.HasValue || !Drop.HasValue) return "-";
        return Short(Pickup.Value) + " to " + Short(Drop.Value);
    }

    public static string ReturnText()
    {
        return Drop.HasValue ? Long(Drop.Value) : "-";
    }
}
