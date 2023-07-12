namespace SwizlyPeasy.Test.IntegrationTest;

public static class Constants
{
    public const string GatewayRootResponse =
        "{\"ServiceDiscoveryAddress\":\"http://consul:8500\",\"StatusCheckedAt\":\"2023-07-12T22:21:33.8033362+00:00\",\"Clusters\":[{\"ClusterId\":\"DemoAPI\",\"Healthy\":true,\"Destinations\":[{\"Healthy\":true,\"Id\":\"DemoAPI-1\",\"Address\":\"http://demo:80\"}]}]}";
}