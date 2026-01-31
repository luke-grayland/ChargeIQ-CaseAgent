using OpenAI.Chat;

namespace CaseAgent.Tools;

public static class Tools
{
    public static string GetDeploymentInfo(int deploymentId)
    {
        Console.WriteLine($"Getting deployment data for deployment {deploymentId}");
        return "User travelled to Paris on 14th July 2025 with a virtual card from Amex with card number 1234567890123456 and expiry date 12/25.";
    }
    
    public static string GetInvoiceData(int invoiceId)
    {
        Console.WriteLine($"Getting invoice data for invoice {invoiceId}");
        return "Invoice was received on 20th July 2025 for a total of $1000.00.";
    }

    public static readonly ChatTool GetGetInvoiceDataTool = ChatTool.CreateFunctionTool(
        functionName: nameof(GetInvoiceData),
        functionDescription: "Get invoice data by invoice ID",
        functionParameters: BinaryData.FromBytes("""
             {
                 "type": "object",
                 "properties": {
                     "invoiceId": {
                         "type": "integer",
                         "description": "The ID representing the invoice, which holds all the information about the virtual card and associated spend information"
                     }
                 },
                 "required": [ "invoiceId" ]
             }
             """u8.ToArray())
    );
    
    public static readonly ChatTool GetDeploymentInfoTool = ChatTool.CreateFunctionTool(
        functionName: nameof(GetDeploymentInfo),
        functionDescription: "Get the virtual card and associated spend information by deployment ID",
        functionParameters: BinaryData.FromBytes("""
             {
                 "type": "object",
                 "properties": {
                     "deploymentId": {
                         "type": "integer",
                         "description": "The ID representing the deployment, which holds all the information about the virtual card and associated spend information"
                     }
                 },
                 "required": [ "deploymentId" ]
             }
             """u8.ToArray())
    );
}