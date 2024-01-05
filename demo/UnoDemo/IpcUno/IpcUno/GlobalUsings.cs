global using System.Collections.Immutable;
global using System.Windows.Input;

global using Microsoft.Extensions.DependencyInjection;

global using Windows.Networking.Connectivity;
global using Windows.Storage;

global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
global using Microsoft.Extensions.Options;

global using IpcUno.Business.Models;
global using IpcUno.Presentation;

#if MAUI_EMBEDDING
global using IpcUno.MauiControls;
#endif
global using Uno.UI;

global using Windows.ApplicationModel;

global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;

global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
