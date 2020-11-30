namespace Skywalker.Lightning.Cluster
{
    public interface ILightningDescriptorResolver
    {

        LightningDescriptor ResolveLightningDescriptor(string serviceName);
    }
}
