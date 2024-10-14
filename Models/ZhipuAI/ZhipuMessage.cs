using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basement.Models.ZhipuAI
{
    public struct ToolCalls
    {
        string id;
        string type;
        object function;
        string name;
        string arguments;
    }
    public abstract class ZhipuMessage
    {
        public abstract string role { get; }
        public abstract string content { get; set; }

        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public virtual string ToFormatString()
        {
            Type type = GetType();

            string formattedString = "";

            foreach (var property in type.GetProperties())
            {
                formattedString = formattedString + property.Name + ":";
                object value = property.GetValue(this) ?? "NULL";
                formattedString = formattedString + value.ToString() + "\n";
            }

            return formattedString;
        }
    }

    public class SystemMessage : ZhipuMessage
    {
        public override string role { get; } = "system";
        public override string content { get; set; } = "";

        public SystemMessage() { }

        public SystemMessage(string _content = "")
        {
            content = _content;
        }
    }

    public class UserMessage : ZhipuMessage
    {
        public override string role { get; } = "user";
        public override string content { get; set; } = "";
        public UserMessage() { }
        public UserMessage(string _content = "")
        {
            content = _content;
        }
    }

    public class AssistantMessage : ZhipuMessage
    {
        public override string role { get; } = "assistant";
        public override string content { get; set; } = "";

        ToolCalls? tool_calls = new ToolCalls();

        public AssistantMessage() { }
        public AssistantMessage(ToolCalls toolCalls)
        {
            tool_calls = toolCalls;
            content = string.Empty;
        }

        public AssistantMessage(string _content)
        {
            content = _content;
            tool_calls = null;
        }
    }

    public class ToolMessage : ZhipuMessage
    {
        public override string role { get; } = "tool";
        public override string content { get; set; } = "";
        [Required]
        public string tool_calls_id { get; set; }

        public ToolMessage() { }

        public ToolMessage(string _content, string _tool_calls_id)
        {
            content = _content;
            tool_calls_id = _tool_calls_id;
        }
    }
}
