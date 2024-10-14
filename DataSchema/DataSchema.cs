using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Basement.DataSchema
{

    public abstract class DataSchema
    {
    }

    public class SampleSchema : DataSchema
    {
        [Required]
        [Description("The ID of a person.")]
        public string ID { get; set; } = "-1";

        [Required]
        [Description("The Name of a person.")]
        public string Name { get; set; } = "Charname";

        public SampleSchema() { }
    }

    public class SampleList : DataSchema
    {
        [Required]
        [Description("A list of people.")]
        public List<SampleSchema> entities { get; set; } = new List<SampleSchema>() { new SampleSchema() };
    }


}
