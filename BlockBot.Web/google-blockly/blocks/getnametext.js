goog.require('Blockly.Blocks');
goog.require('Blockly');

Blockly.defineBlocksWithJsonArray(
    [
        {
            "type": "name_text_block",
            "message0": "Send Message %1 %2 if %3 then %4 else %5",
            "args0": [
                {
                    "type": "field_input",
                    "name": "message_text",
                    "text": "Hello! Can you please tell me your first and last name?",
                },
                {
                    "type": "input_dummy"
                },
                {
                    "type": "input_value",
                    "name": "if"
                },
                {
                    "type": "field_input",
                    "name": "then send message",
                    "text": "Sorry, I didn't catch that - can you please tell me your first and last name?"
                },
                {
                    "type": "field_input",
                    "name": "else",
                    "text": "Great!"
                }
            ],
            "previousStatement": null,
            "nextStatement": null,
            "colour": 230,
            "tooltip": "Ask for user first and last name (SMS)",
            "helpUrl": ""
        }
    ]);

/*{
        "type": "send_sms",
        "message0": "Send Message %1",
        "args0": [
            {
                "type": "input_value",
                "name": "message_text",
                "check": "String"
            }
        ],
        "previousStatement": null,
        "nextStatement": null,
        "colour": 230,
        "tooltip": "Send a message to the user",
        "helpUrl": ""
    }*/