using RohdeSchwarz.RsInstrument;

namespace Monitoring;

public class HMCDeviceWrapper
{
    private readonly RsInstrument _device;

    public HMCDeviceWrapper(string address)
    {
        this._device = new RsInstrument(address, true, true, "SelectVisa=Socket");
        _device.InstrumentStatusChecking = true;
        _device.ClearStatus();
        _device.Reset();
        _device.WriteStringWithOpc("VIEW:NUMeric:PAGE1:CELL5:FUNCtion FU");
    }

    public float GetPower()
    {
        var newMeasurement = _device.QueryString("CHANnel1:MEASurement:DATA?");
        return float.Parse(newMeasurement);
    }
}