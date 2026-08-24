using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadMD.Services;

public interface ISingleInstanceService
{
    bool TryAcquireInstance();
    void StartListening(Action<string> onFilePathReceived);
    Task SendFilePathToRunningInstanceAsync(string filePath);
    void Release();
}

public class SingleInstanceService : ISingleInstanceService, IDisposable
{
    private const string MutexName = @"Global\ReadMD_SingleInstance_Mutex_v1";
    private const string PipeName = "ReadMD_IPC_Pipe_v1";

    private Mutex? _mutex;
    private CancellationTokenSource? _cancellationTokenSource;

    public bool TryAcquireInstance()
    {
        try
        {
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            System.Diagnostics.Debug.WriteLine($"[SingleInstance] TryAcquireInstance: createdNew={createdNew}");
            return createdNew;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SingleInstance] TryAcquireInstance exception: {ex.Message}");
            return true; // Fallback to allowing instance
        }
    }

    public void StartListening(Action<string> onFilePathReceived)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => ListenForConnectionsAsync(onFilePathReceived, _cancellationTokenSource.Token));
    }

    private async Task ListenForConnectionsAsync(Action<string> onFilePathReceived, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync(cancellationToken);

                using var reader = new StreamReader(server, Encoding.UTF8);
                var filePath = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"[SingleInstance] Received file path via IPC: {filePath}");
                    onFilePathReceived(filePath);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SingleInstance IPC error: {ex.Message}");
            }
        }
    }

    public async Task SendFilePathToRunningInstanceAsync(string filePath)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[SingleInstance] Sending file path via IPC: {filePath}");
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            await client.ConnectAsync(2000);

            using var writer = new StreamWriter(client, Encoding.UTF8);
            await writer.WriteAsync(filePath);
            await writer.FlushAsync();
            System.Diagnostics.Debug.WriteLine($"[SingleInstance] File path sent successfully");
        }
        catch (TimeoutException)
        {
            System.Diagnostics.Debug.WriteLine($"[SingleInstance] Timeout connecting to running instance");
            throw new InvalidOperationException("Не удалось подключиться к запущенному экземпляру приложения.");
        }
    }

    public void Release()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
    }

    public void Dispose()
    {
        Release();
    }
}
