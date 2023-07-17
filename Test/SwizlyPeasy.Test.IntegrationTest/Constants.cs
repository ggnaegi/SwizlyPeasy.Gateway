namespace SwizlyPeasy.Test.IntegrationTest;

public static class Constants
{
    public const string GatewayRootResponse =
        "{\"ServiceDiscoveryAddress\":\"http://localhost:8500\",\"StatusCheckedAt\":\"2023-07-17T22:18:29.4745635+02:00\",\"Clusters\":[{\"ClusterId\":\"DemoAPI\",\"Healthy\":true,\"Destinations\":[{\"Healthy\":true,\"Id\":\"DemoAPI-1\",\"Address\":\"http://127.0.0.1:8020\"},{\"Healthy\":true,\"Id\":\"DemoAPI-2\",\"Address\":\"http://127.0.0.1:8030\"}]}]}";

    public const string GatewayConfigWithPathSet =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route3\":{\"ClusterId\":\"DemoAPI\",\"Match\":{\"Path\":\"/api/v1/demo/weather-anonymous\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"},{\"PathSet\":\"/super-test\"}]}}}";
}