global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Composition;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Linq;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using dotnetCampus.Ipc.CodeAnalysis.Core;
global using dotnetCampus.Ipc.CodeAnalysis.Utils;
global using dotnetCampus.Ipc.CompilerServices.Attributes;
global using dotnetCampus.Ipc.Core.ComponentModels;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;

global using static dotnetCampus.Ipc.CodeAnalysis.Core.Diagnostics;
global using static dotnetCampus.Ipc.CodeAnalysis.Utils.SyntaxNameGuesser;
