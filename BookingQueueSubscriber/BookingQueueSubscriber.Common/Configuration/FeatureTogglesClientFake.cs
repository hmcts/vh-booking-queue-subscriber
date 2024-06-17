using System.Diagnostics.CodeAnalysis;

namespace BookingQueueSubscriber.Common.Configuration;

[ExcludeFromCodeCoverage]
public class FeatureTogglesClientFake : IFeatureToggles
{
    public bool PostMayTemplateToggle { get; set; } = false;
    
    public bool EjudFeatureToggleValue { get; set; } = true;

    public bool UsePostMay2023Template()
    {
        return PostMayTemplateToggle;
    }

    public bool EjudFeatureToggle()
    {
        return EjudFeatureToggleValue;
    }
}