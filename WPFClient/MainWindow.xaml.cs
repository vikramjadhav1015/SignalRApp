using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    HubConnection connection;

    //constructor
    public MainWindow()
    {
        InitializeComponent();

        //setup the connection
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7266/chathub")
            .WithAutomaticReconnect()
            .Build();

        //Reconnecting Event
        //if connection issue happens
        //execute when trying to reconnect the hub
        connection.Reconnecting += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage); //listbox messages
            });

            return Task.CompletedTask;
        };

        connection.Reconnected += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Reconnected to the server";
                messages.Items.Clear();
                messages.Items.Add(newMessage);
            });

            return Task.CompletedTask;
        };


        connection.Closed += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Connection Closed";
                messages.Items.Add(newMessage);

                openConnection.IsEnabled= true; //to enable open connection button
                sendMessage.IsEnabled= false; // to disable send message button
            });

            return Task.CompletedTask;
        };
    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {
        //when message arrived at Hub this message will receive the message
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message}";
                messages.Items.Add(newMessage);
            });

        });


        try
        {
            await connection.StartAsync();
            messages.Items.Add("Connection Started");
            openConnection.IsEnabled= false;
            sendMessage.IsEnabled= true;
        }
        catch(Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await connection.InvokeAsync("SendMessage",
                "WPF Client", messageInput.Text);
        }
        catch (Exception ex)
        { 
            messages.Items.Add(ex.Message);
        }
    }
}
