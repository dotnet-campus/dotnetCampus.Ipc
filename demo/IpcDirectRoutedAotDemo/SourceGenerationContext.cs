﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IpcDirectRoutedAotDemo;

[JsonSerializable(typeof(string))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
