namespace SwizlyPeasy.Test.IntegrationTest;

public static class Constants
{
    public const string GatewayRootResponseNoServices =
        "{\"ServiceDiscoveryAddress\":\"http://localhost:8500\",\"StatusCheckedAt\":\"2023-07-12T22:21:33.8033362+00:00\",\"Clusters\":[]}";

    public const string GatewayConfigWithPathSet =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route3\":{\"ClusterId\":\"DemoAPI\",\"Match\":{\"Path\":\"/api/v1/demo/weather-anonymous\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"},{\"PathSet\":\"/super-test\"}]}}}";
}