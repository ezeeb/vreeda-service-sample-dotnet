namespace VreedaServiceSampleDotNet.Models;

public class DevicesRequestModel : Dictionary<string, DeviceRequestModel>;

public class DeviceRequestModel
{
    public DeviceStateRequestModel? states { get; set; }
}

public class DeviceStateRequestModel
{
    public bool? on { get; set; }
    public float? v { get; set; }
    public string? program { get; set; }
    public string? pattern { get; set; }
    public bool? playing { get; set; }
}

public class DeviceTagsModel
{
    public int? deviceTypeId { get; set; }
    public string? deviceManufacturer { get; set; }
    public string? customDeviceName { get; set; }
    public string? deviceTypeURN { get; set; }
}

public class DevicesResponseModel : Dictionary<string, DeviceResponseModel>;

public class DeviceResponseModel
{
    public DeviceTagsModel? tags { get; set; }

    public DeviceStateResponseModel? states { get; set; }

    public DeviceConfigModel? config { get; set; }

    public DeviceStateValueResponseModel<bool>? connected { get; set; }
}

public class DeviceStateResponseModel
{
    public DeviceStateValueResponseModel<bool>? on { get; set; }
    
    public DeviceStateValueResponseModel<double>? v { get; set; }
    
    public DeviceStateValueResponseModel<string>? program { get; set; }
    
    public DeviceStateValueResponseModel<string>? pattern { get; set; }
    
    public DeviceStateValueResponseModel<bool>? playing { get; set; }
}

public class DeviceStateValueResponseModel<T>
{
    public T? value { get; set; }
    public DateTime updatedAt { get; set; }
}

public class DeviceConfigModel
{
    public string? sn { get; set; }

    public string? fwVer { get; set; }

    public string? fwId { get; set; }

    public string? mac { get; set; }

    public string? deviceId { get; set; }

    public string? app { get; set; }

    public int? assignedToUser { get; set; }

    public string? sgtin { get; set; }
}