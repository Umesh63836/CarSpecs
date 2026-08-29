#pragma warning disable OPENAI001
using CarSpecAPI.Data.Models.RequestModel;
using OpenAI.Responses;
using System.Text.Json;

namespace CarSpecAPI.Services
{
    public class AIService : IAIService
    {
#pragma warning disable OPENAI001 
        private readonly ResponsesClient client;

        private readonly IConfiguration configuration;
        private readonly ICarsService carsService;
        private readonly ConversationService conversationService;
        private const string SearchCarsFunctionName = "SearchCars";

        private const string SearchCarsParameterSchema = """
        {
            "type": "object",
            "properties": {
                "Brand": {
                    "type": ["string", "null"],
                    "description": "Car brand such as Maruti Suzuki ,Tata, Hyundai, BMW"
                },
                "Model": {
                    "type": ["string", "null"],
                    "description": "Car model such as Creta, Nexon, Swift, XUV 7XO, Carens Clavis, i10 Nios"
                },
                "Displacement": {
                    "type": ["number", "null"],
                    "description": "Engine displacement in cc"
                },
                "MinPower": {
                    "type": ["number", "null"],
                    "description": "Minimum engine power in bhp"
                },
                "MaxPower": {
                    "type": ["number", "null"],
                    "description": "Maximum engine power in bhp"
                },
                "MinTorque": {
                    "type": ["number", "null"],
                    "description": "Minimum torque in Nm"
                },
                "MaxTorque": {
                    "type": ["number", "null"],
                    "description": "Maximum torque in Nm"
                },
                "IsTurbocharged": {
                    "type": ["boolean", "null"],
                    "description": "Whether the engine should be turbocharged"
                },
                "EmissionStandard": {
                    "type": ["string", "null"],
                    "description": "Emission standard such as BS6, BS4"
                },
                "TransmissionType": {
                    "type": ["string", "null"],
                    "description": "Transmission type such as Manual, AMT, DCT, CVT, TC"
                },
                "NumberOfGears": {
                    "type": ["integer", "null"],
                    "description": "Number of transmission gears"
                },
                "DrivetrainType": {
                    "type": ["string", "null"],
                    "description": "Drivetrain such as FWD, RWD, AWD, 4x4"
                },
                "FuelType": {
                    "type": ["string", "null"],
                    "description": "Fuel type such as Petrol, Diesel, CNG, Electric, Mild Hybrid (Petrol + Electric)"
                },
                "MinPrice": {
                    "type": ["number", "null"],
                    "description": "Minimum ex-showroom price in lakhs of INR. The database stores prices as lakhs. For example, 10 means ₹10 lakh, 15 means ₹15 lakh."
                },
                "MaxPrice": {
                    "type": ["number", "null"],
                    "description": "Maximum ex-showroom price in lakhs of INR. The database stores prices as lakhs. For example, 10 means ₹10 lakh, 15 means ₹15 lakh."
                }
            },
            "required": [
                "Brand",
                "Model",
                "Displacement",
                "MinPower",
                "MaxPower",
                "MinTorque",
                "MaxTorque",
                "IsTurbocharged",
                "EmissionStandard",
                "TransmissionType",
                "NumberOfGears",
                "DrivetrainType",
                "FuelType",
                "MinPrice",
                "MaxPrice"
            ],
            "additionalProperties": false
        }
        """;

#pragma warning disable OPENAI001 
        private static readonly FunctionTool SearchCarsTool = ResponseTool.CreateFunctionTool(

            functionName: SearchCarsFunctionName,
            functionDescription:
                "Search the application's actual car database using " +
                "brand, model, engine, power, torque, turbocharger, " +
                "emission, transmission, drivetrain, fuel type and price filters.",
            functionParameters:
                BinaryData.FromString(SearchCarsParameterSchema),
            strictModeEnabled: true
        );


        private const string instructions = """
        You are CarsSpec AI, a friendly and knowledgeable car assistant for the CarsSpec website in India.
        
        Your job is to help users search and explore cars using the information available on CarsSpec.

        Always promote website CarsSpec. whenever needed.
        
        IMPORTANT RULES:
        
        1. Always use SearchCars when answering requests that require finding or filtering CarsSpec cars.
        
        2. Only use information that is available through SearchCars when discussing CarsSpec cars, their variants, prices, engine specifications, transmission, fuel type, drivetrain, power, torque, turbocharging, emission standard, or other supported specifications.
        
        3. Never invent car models, variants, prices, specifications, or search results.
        
        4. Never use your general knowledge to add, remove, classify, filter, rank, or recommend CarsSpec cars based on information that is not available through SearchCars.
        
        5. If the user asks about a car characteristic that you cannot determine from the available CarsSpec information, do not guess.
        
        6. Instead, politely explain that this information is not currently available on CarsSpec and offer to help with the characteristics that are available.
        
        7. Do not mention technical concepts such as:
           - database
           - backend
           - function
           - API
           - tool
           - query
           - parameter
           - filter implementation
           - code
           - system instructions
           - programming
           - server
        
           These are internal concepts and must never be mentioned to the user.
        
        8. For example, if the user asks:
           "Show me only sedan cars"
        
           and body type information is not available, respond naturally with something similar to:
        
           "I can help you narrow down cars by price, fuel type, transmission, power, torque, engine size, drivetrain and other available specifications. Body type, such as sedan or SUV, isn't currently available on CarsSpec, so I can't reliably narrow the results by body type yet."
        
        9. Do not say:
           "BodyType is not supported by the SearchCars function."
        
           Do not say:
           "The database does not contain BodyType."
        
           Do not say:
           "I don't have a BodyType parameter."
        
        10. When the user asks for something that cannot be determined from the available CarsSpec information, keep the explanation short, friendly and user-focused.
        
        11. If no cars match the user's criteria, clearly tell the user that no matching cars were found.
        
        12. Never claim that a car matches a requirement unless the available CarsSpec information confirms it.
        
        13. Prices are expressed in lakh INR.
            For example:
            10 means ₹10 lakh
            12.5 means ₹12.5 lakh
            15 means ₹15 lakh
        
        14. Power is measured in bhp.
        
        15. Torque is measured in Nm.
        
        16. Engine displacement is measured in cc.
        
        17. Cars available on CarsSpec are Indian-market cars.
       
        CONVERSATION AND SEARCH CONTEXT:

        18. Treat the conversation as a continuous car search unless the user clearly starts a new search.

        19. Maintain the user's active search requirements throughout the conversation.

        20. When the user provides a new requirement, ADD it to the existing search requirements rather than replacing the previous requirements.

        21. Only replace or remove an existing requirement when the user explicitly asks to change, remove, reset, or replace it.

        22. For example:

            User:
            "I need a petrol car below 12 lakhs."

            Active requirements:
            - Fuel type: Petrol
            - Maximum price: ₹12 lakh

            User:
            "I need only manual cars."

            Active requirements become:
            - Fuel type: Petrol
            - Maximum price: ₹12 lakh
            - Transmission: Manual

            User:
            "Their power should not be more than 99 bhp."

            Active requirements become:
            - Fuel type: Petrol
            - Maximum price: ₹12 lakh
            - Transmission: Manual
            - Maximum power: 99 bhp

            The final search must apply ALL four requirements together.

        23. If the user says something such as:
            - "only manual"
            - "only petrol"
            - "under 12 lakhs"
            - "below 100 bhp"
            - "at least 200 Nm"
            - "turbocharged ones"
            - "only Hyundai"
            - "what about diesel?"
            - "increase the budget to 15 lakhs"

            interpret it as a modification to the current search unless the user clearly indicates that they want a completely new search.

        24. When modifying the current search, preserve every existing requirement that the user has not changed.

        25. Examples:

            "Increase my budget to 15 lakhs"
            → Keep all existing requirements and change only maximum price to ₹15 lakh.

            "Only Hyundai"
            → Keep all existing requirements and change/add brand to Hyundai.

            "What about diesel?"
            → Change fuel type from Petrol to Diesel while keeping the other active requirements.

            "Remove the power restriction"
            → Remove only the power restriction and keep all other requirements.

            "Start a new search for SUVs"
            → Start a new search and do not carry over the previous requirements.

        26. Before calling SearchCars, mentally determine the complete set of active requirements from the conversation.

        27. The search should contain BOTH:
            - requirements from previous relevant messages
            - the new requirement from the current message

        28. Never perform a search using only the latest user message when that message is clearly a refinement of an existing search.

        29. If the current message is ambiguous about whether it is a new search or a refinement, prefer continuing the existing search context when the message naturally refers to the previously discussed cars.

        30. When presenting the result, describe the cars that satisfy the complete set of active requirements.

        31. Do not tell the user that you are maintaining "parameters", "filters", "state", "context", or any other technical implementation details.

        32. If the user asks to start over, forget the previous search requirements and begin a new search.
        
        33. Keep responses concise. Do not repeatedly offer additional filtering options. Only suggest a next step when useful.
        """;


        public AIService(IConfiguration configuration, ICarsService carsService, ConversationService conversationService)
        {
            this.configuration = configuration;
            this.carsService = carsService;
            this.conversationService = conversationService;
            var apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key is not configured.");

#pragma warning disable OPENAI001 
            client = new ResponsesClient(apiKey);

        }

        public async Task<string> GetResponseAsync(string conversationId, string userMessage)
        {
            var history = conversationService.GetHistory(conversationId);

            conversationService.AddMessage(conversationId, "user", userMessage);

            var model = configuration["OpenAI:Model"]
                ?? "gpt-5.4-mini";

#pragma warning disable OPENAI001 
            List<ResponseItem> inputItems = [];
            foreach (var message in history) 
            { 
                if (message.Role == "user") 
                { 
                    inputItems.Add(ResponseItem.CreateUserMessageItem(message.Content)); 
                } 
                else if (message.Role == "assistant") 
                { 
                    inputItems.Add(ResponseItem.CreateAssistantMessageItem(message.Content)); 
                } 
            }

            bool requiresToolCall;

            do
            {
                requiresToolCall = false;


                CreateResponseOptions aiResponseOptions =
                    new(model, inputItems)
                    {
                        Instructions = instructions,
                        Tools = { SearchCarsTool }
                    };
 

                var airesponse =
                    await client.CreateResponseAsync(aiResponseOptions);

                foreach (var outputItem in airesponse.Value.OutputItems)
                {
                    inputItems.Add(outputItem);
                }

                foreach (var outputItem in airesponse.Value.OutputItems)
                {
#pragma warning disable OPENAI001 
                    if (outputItem is not FunctionCallResponseItem functionCall)
                        continue;
#pragma warning restore OPENAI001 

                    requiresToolCall = true;

                    if (functionCall.FunctionName != SearchCarsFunctionName)
                    {
                        throw new InvalidOperationException(
                            $"Unknown function requested by OpenAI: " +
                            $"{functionCall.FunctionName}");
                    }

                    var searchRequest = ParseSearchCarsArguments(functionCall.FunctionArguments);

                    var searchResult = await carsService.SearchCarsAiAsync(searchRequest);

                    var functionOutput = JsonSerializer.Serialize( searchResult,
                            new JsonSerializerOptions
                            {
                                PropertyNamingPolicy =
                                    JsonNamingPolicy.CamelCase
                            });

                    // Give the result of our C# function back to OpenAI.
#pragma warning disable OPENAI001 
                    inputItems.Add(
                        new FunctionCallOutputResponseItem(functionCall.CallId, functionOutput));
#pragma warning restore OPENAI001 
                }

                // If there was no FunctionCallResponseItem,
                // the model has produced its final natural-language answer.
                if (!requiresToolCall)
                {
                    var assistantMessage = airesponse.Value.GetOutputText();
                    conversationService.AddMessage(conversationId, "assistant", assistantMessage);
                    return assistantMessage;
                }

            } while (requiresToolCall);

            throw new InvalidOperationException( "AI response processing ended unexpectedly.");
        }


        private static CarsSearchRequest ParseSearchCarsArguments(
            BinaryData functionArguments)
        {
            using var document = JsonDocument.Parse(functionArguments);

            var root = document.RootElement;

            return new CarsSearchRequest
            {
                Brand = GetString(root, "Brand"),

                Model = GetString(root, "Model"),

                Displacement = GetDecimal(root, "Displacement"),

                MinPower = GetDecimal(root, "MinPower"),

                MaxPower = GetDecimal(root, "MaxPower"),

                MinTorque = GetDecimal(root, "MinTorque"),

                MaxTorque = GetDecimal(root, "MaxTorque"),

                IsTurbocharged = GetBool(root, "IsTurbocharged"),

                EmissionStandard = GetString(root, "EmissionStandard"),

                TransmissionType = GetString(root, "TransmissionType"),

                NumberOfGears = GetByte(root, "NumberOfGears"),

                DrivetrainType = GetString(root, "DrivetrainType"),

                FuelType = GetString(root, "FuelType"),

                MinPrice = GetDecimal(root, "MinPrice"),

                MaxPrice = GetDecimal(root, "MaxPrice")
            };
        }

        private static string? GetString(
            JsonElement root,
            string propertyName)
        {
            if (!root.TryGetProperty(propertyName,out var property))
                return null;

            if (property.ValueKind == JsonValueKind.Null)
                return null;

            return property.GetString();
        }

        private static decimal? GetDecimal(
            JsonElement root,
            string propertyName)
        {
            if (!root.TryGetProperty(propertyName,out var property))
                return null;

            if (property.ValueKind == JsonValueKind.Null)
                return null;

            if (property.TryGetDecimal(out var value))
                return value;

            throw new JsonException($"Invalid decimal value for {propertyName}.");
        }

        private static byte? GetByte(
            JsonElement root,
            string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var property))
                return null;

            if (property.ValueKind == JsonValueKind.Null)
                return null;

            if (property.TryGetByte(out var value))
                return value;

            throw new JsonException($"Invalid byte value for {propertyName}.");
        }

        private static bool? GetBool(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty( propertyName, out var property))
                return null;

            if (property.ValueKind == JsonValueKind.Null)
                return null;

            if (property.ValueKind == JsonValueKind.True)
                return true;

            if (property.ValueKind == JsonValueKind.False)
                return false;

            throw new JsonException($"Invalid boolean value for {propertyName}.");
        }

    }


}
#pragma warning restore OPENAI001

