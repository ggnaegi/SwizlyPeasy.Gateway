namespace SwizlyPeasy.Test.IntegrationTest;

public static class Constants
{
    public const string GatewayRootResponse =
        "{\"ServiceDiscoveryAddress\":\"http://localhost:8500\",\"StatusCheckedAt\":\"2023-07-19T03:19:22.6738087+02:00\",\"Clusters\":[{\"ClusterId\":\"DemoAPI\",\"Healthy\":true,\"Destinations\":[{\"Healthy\":true,\"Id\":\"DemoAPI-1\",\"Address\":\"http://127.0.0.1:8020\"},{\"Healthy\":true,\"Id\":\"DemoAPI-2\",\"Address\":\"http://127.0.0.1:8030\"}]},{\"ClusterId\":\"FakeAPI\",\"Healthy\":false,\"Destinations\":[{\"Healthy\":false,\"Id\":\"FakeAPI-1\",\"Address\":\"http://127.0.0.1:8040\"}]}]}";

    public const string GatewayConfigWithPathSet =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route3\":{\"ClusterId\":\"DemoAPI\",\"Match\":{\"Path\":\"/newpath\"},\"Transforms\":[{\"PathSet\":\"/api/v1/demo/weather-anonymous\"}]}}}";

    public const string GatewayBaseConfig =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route3\":{\"ClusterId\":\"DemoAPI\",\"Match\":{\"Path\":\"/api/v1/demo/weather-anonymous\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]}}}";
}