using Microsoft.AspNetCore.Mvc;

namespace ValidateIdentityBeforeCreationFunction.Extensions
{
    public static class ResponseExtensions
    {
        public static ContentResult Continue() => new()
        {
            Content = """
                {
                  "data": {
                    "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
                    "actions": [
                      {
                        "@odata.type": "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"
                      }
                    ]
                  }
                }
                """,
            ContentType = "application/json",
            StatusCode = 200
        };

        public static ContentResult Block(string message) => new()
        {
            Content = $$"""
            {
              "data": {
                "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
                "actions": [
                  {
                    "@odata.type": "microsoft.graph.attributeCollectionSubmit.showBlockPage",
                    "message": "{{message}}"
                  }
                ]
              }
            }
            """,

            ContentType = "application/json",
            StatusCode = 200
        };
    }
}