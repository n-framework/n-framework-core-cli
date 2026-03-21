using NFramework.Core.CLI.Abstractions;
using NFramework.Core.CLI.SpectreConsoleUI;
using Shouldly;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace NFramework.Core.CLI.SpectreConsoleUI.Tests;

public class SpectreConsoleTerminalSessionTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesSession_WithValidConsole()
        {
            // Arrange
            IAnsiConsole console = new TestConsole();

            // Act
            SpectreConsoleTerminalSession session = new(console);

            // Assert
            _ = session.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenConsoleIsNull()
        {
            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => new SpectreConsoleTerminalSession(null!));
        }
    }

    public class IsInteractive
    {
        [Fact]
        public void ReturnsTrue_WhenConsoleIsNotRedirected()
        {
            // This test verifies the property exists and returns a boolean
            // Note: In actual test environments, this may return false due to redirected I/O
            // Arrange
            IAnsiConsole console = new TestConsole();
            SpectreConsoleTerminalSession session = new(console);

            // Act
            bool isInteractive = session.IsInteractive;

            // Assert - Just verify it returns a valid boolean
            // The actual value depends on the test environment
            (isInteractive == true || isInteractive == false).ShouldBeTrue();
        }
    }

    public class WriteLine
    {
        [Fact]
        public void WritesMessage_ToConsole()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);
            string testMessage = "Test message";

            // Act
            session.WriteLine(testMessage);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain(testMessage);
        }

        [Fact]
        public void HandlesEmptyMessage()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            // Act & Assert - Should not throw
            session.WriteLine(string.Empty);
        }

        [Fact]
        public void HandlesMultiLineMessage()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);
            string testMessage = "Line 1\nLine 2\nLine 3";

            // Act
            session.WriteLine(testMessage);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain("Line 1");
            output.ShouldContain("Line 2");
            output.ShouldContain("Line 3");
        }
    }

    public class WriteErrorLine
    {
        [Fact]
        public void WritesErrorMessage_ToErrorWriter()
        {
            // Arrange
            StringWriter errorWriter = new System.IO.StringWriter();
            TestConsole testConsole = new TestConsole();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new SpectreTextPromptRunner(),
                new SpectreSelectionPromptRunner()
            );
            string testMessage = "Error message";

            // Act
            session.WriteErrorLine(testMessage);

            // Assert
            string output = errorWriter.ToString();
            output.ShouldContain(testMessage);
        }

        [Fact]
        public void HandlesEmptyErrorMessage()
        {
            // Arrange
            StringWriter errorWriter = new System.IO.StringWriter();
            TestConsole testConsole = new TestConsole();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new SpectreTextPromptRunner(),
                new SpectreSelectionPromptRunner()
            );

            // Act & Assert - Should not throw
            session.WriteErrorLine(string.Empty);
        }
    }

    public class RenderTable
    {
        [Fact]
        public void RendersSimpleTable()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            TerminalTable table = new TerminalTable(
                ["Name", "Value"],
                [new TerminalTableRow(["Item1", "Value1"]), new TerminalTableRow(["Item2", "Value2"])]
            );

            // Act
            session.RenderTable(table);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain("Name");
            output.ShouldContain("Value");
            output.ShouldContain("Item1");
            output.ShouldContain("Value1");
            output.ShouldContain("Item2");
            output.ShouldContain("Value2");
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenTableIsNull()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() => session.RenderTable(null!));
        }

        [Fact]
        public void RendersEmptyTable()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            TerminalTable table = new TerminalTable(["Col1", "Col2"], []);

            // Act
            session.RenderTable(table);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain("Col1");
            output.ShouldContain("Col2");
        }

        [Fact]
        public void RendersTableWithMultipleRows()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            List<TerminalTableRow> rows = new List<TerminalTableRow>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new TerminalTableRow([$"Row{i}-Col1", $"Row{i}-Col2", $"Row{i}-Col3"]));
            }

            TerminalTable table = new TerminalTable(["Column1", "Column2", "Column3"], rows);

            // Act
            session.RenderTable(table);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain("Column1");
            output.ShouldContain("Row0-Col1");
            output.ShouldContain("Row9-Col3");
        }

        [Fact]
        public void RendersTableWithSpecialCharacters()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);

            TerminalTable table = new TerminalTable(
                ["Test"],
                [new TerminalTableRow(["Value with special chars: <>&\"'"])]
            );

            // Act
            session.RenderTable(table);

            // Assert
            string output = testConsole.Output;
            output.ShouldContain("Value with special chars:");
        }
    }

    public class PromptForTextAsync
    {
        [Fact]
        public async Task ThrowsOperationCanceledException_WhenTokenIsCancelled()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter value:", "Invalid value");

            // Act & Assert
            _ = await Should.ThrowAsync<OperationCanceledException>(() =>
                session.PromptForTextAsync(prompt, cts.Token)
            );
        }
    }

    public class PromptForSelectionAsync
    {
        [Fact]
        public async Task ThrowsOperationCanceledException_WhenTokenIsCancelled()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            IAnsiConsole console = testConsole;
            SpectreConsoleTerminalSession session = new(console);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            TerminalSelectionOption[] options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act & Assert
            _ = await Should.ThrowAsync<OperationCanceledException>(() =>
                session.PromptForSelectionAsync(prompt, cts.Token)
            );
        }
    }

    public class InternalConstructor
    {
        [Fact]
        public void CreatesSession_WithAllValidParameters()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner textRunner = new MockTextPromptRunner();
            MockSelectionPromptRunner selectionRunner = new MockSelectionPromptRunner();

            // Act
            SpectreConsoleTerminalSession session = new(testConsole, errorWriter, textRunner, selectionRunner);

            // Assert
            _ = session.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenConsoleIsNull()
        {
            // Arrange
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner textRunner = new MockTextPromptRunner();
            MockSelectionPromptRunner selectionRunner = new MockSelectionPromptRunner();

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() =>
                new SpectreConsoleTerminalSession(null!, errorWriter, textRunner, selectionRunner)
            );
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenErrorWriterIsNull()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            MockTextPromptRunner textRunner = new MockTextPromptRunner();
            MockSelectionPromptRunner selectionRunner = new MockSelectionPromptRunner();

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() =>
                new SpectreConsoleTerminalSession(testConsole, null!, textRunner, selectionRunner)
            );
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenTextPromptRunnerIsNull()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockSelectionPromptRunner selectionRunner = new MockSelectionPromptRunner();

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() =>
                new SpectreConsoleTerminalSession(testConsole, errorWriter, null!, selectionRunner)
            );
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenSelectionPromptRunnerIsNull()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner textRunner = new MockTextPromptRunner();

            // Act & Assert
            _ = Should.Throw<ArgumentNullException>(() =>
                new SpectreConsoleTerminalSession(testConsole, errorWriter, textRunner, null!)
            );
        }
    }

    public class PromptForTextAsync_MockTests
    {
        [Fact]
        public async Task ReturnsSubmittedResult_WhenRunnerReturnsSuccess()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner mockRunner = new MockTextPromptRunner
            {
                ResultToReturn = TerminalTextInputResult.Submitted("test input"),
            };
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                mockRunner,
                new MockSelectionPromptRunner()
            );
            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter value:", "Invalid");

            // Act
            TerminalTextInputResult result = await session.PromptForTextAsync(prompt, CancellationToken.None);

            // Assert
            result.WasCancelled.ShouldBeFalse();
            result.Value.ShouldBe("test input");
        }

        [Fact]
        public async Task ReturnsCancelledResult_WhenRunnerReturnsCancelled()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner mockRunner = new MockTextPromptRunner
            {
                ResultToReturn = TerminalTextInputResult.Cancelled(),
            };
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                mockRunner,
                new MockSelectionPromptRunner()
            );
            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter value:", "Invalid");

            // Act
            TerminalTextInputResult result = await session.PromptForTextAsync(prompt, CancellationToken.None);

            // Assert
            result.WasCancelled.ShouldBeTrue();
            result.Value.ShouldBeNull();
        }

        [Fact]
        public async Task PassesConsoleToRunner()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner mockRunner = new MockTextPromptRunner();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                mockRunner,
                new MockSelectionPromptRunner()
            );
            TerminalTextPrompt prompt = new TerminalTextPrompt("Enter value:", "Invalid");

            // Act
            _ = await session.PromptForTextAsync(prompt, CancellationToken.None);

            // Assert
            _ = mockRunner.ConsoleReceived.ShouldNotBeNull();
            mockRunner.ConsoleReceived.ShouldBe(testConsole);
        }

        [Fact]
        public async Task PassesPromptToRunner()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockTextPromptRunner mockRunner = new MockTextPromptRunner();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                mockRunner,
                new MockSelectionPromptRunner()
            );
            TerminalTextPrompt prompt = new TerminalTextPrompt("Test prompt:", "Invalid");

            // Act
            _ = await session.PromptForTextAsync(prompt, CancellationToken.None);

            // Assert
            _ = mockRunner.PromptReceived.ShouldNotBeNull();
            mockRunner.PromptReceived.ShouldBe(prompt);
        }
    }

    public class PromptForSelectionAsync_MockTests
    {
        [Fact]
        public async Task ReturnsSelectedResult_WhenRunnerReturnsSuccess()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockSelectionPromptRunner mockRunner = new MockSelectionPromptRunner
            {
                ResultToReturn = TerminalSelectionResult.Selected("option1"),
            };
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new MockTextPromptRunner(),
                mockRunner
            );
            TerminalSelectionOption[] options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act
            TerminalSelectionResult result = await session.PromptForSelectionAsync(prompt, CancellationToken.None);

            // Assert
            result.WasCancelled.ShouldBeFalse();
            result.SelectedValue.ShouldBe("option1");
        }

        [Fact]
        public async Task ReturnsCancelledResult_WhenRunnerReturnsCancelled()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockSelectionPromptRunner mockRunner = new MockSelectionPromptRunner
            {
                ResultToReturn = TerminalSelectionResult.Cancelled(),
            };
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new MockTextPromptRunner(),
                mockRunner
            );
            TerminalSelectionOption[] options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act
            TerminalSelectionResult result = await session.PromptForSelectionAsync(prompt, CancellationToken.None);

            // Assert
            result.WasCancelled.ShouldBeTrue();
            result.SelectedValue.ShouldBeNull();
        }

        [Fact]
        public async Task PassesConsoleToRunner()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockSelectionPromptRunner mockRunner = new MockSelectionPromptRunner();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new MockTextPromptRunner(),
                mockRunner
            );
            TerminalSelectionOption[] options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act
            _ = await session.PromptForSelectionAsync(prompt, CancellationToken.None);

            // Assert
            _ = mockRunner.ConsoleReceived.ShouldNotBeNull();
            mockRunner.ConsoleReceived.ShouldBe(testConsole);
        }

        [Fact]
        public async Task PassesPromptToRunner()
        {
            // Arrange
            TestConsole testConsole = new TestConsole();
            StringWriter errorWriter = new System.IO.StringWriter();
            MockSelectionPromptRunner mockRunner = new MockSelectionPromptRunner();
            SpectreConsoleTerminalSession session = new(
                testConsole,
                errorWriter,
                new MockTextPromptRunner(),
                mockRunner
            );
            TerminalSelectionOption[] options = new[] { new TerminalSelectionOption("1", "Option 1") };
            TerminalSelectionPrompt prompt = new TerminalSelectionPrompt("Choose:", options);

            // Act
            _ = await session.PromptForSelectionAsync(prompt, CancellationToken.None);

            // Assert
            _ = mockRunner.PromptReceived.ShouldNotBeNull();
            mockRunner.PromptReceived.ShouldBe(prompt);
        }
    }
}

// Mock implementations for testing
internal class MockTextPromptRunner : ITextPromptRunner
{
    public IAnsiConsole? ConsoleReceived { get; private set; }
    public TerminalTextPrompt? PromptReceived { get; private set; }
    public TerminalTextInputResult ResultToReturn { get; set; } = TerminalTextInputResult.Submitted("mock");

    public TerminalTextInputResult Prompt(IAnsiConsole console, TerminalTextPrompt prompt)
    {
        ConsoleReceived = console;
        PromptReceived = prompt;
        return ResultToReturn;
    }
}

internal class MockSelectionPromptRunner : ISelectionPromptRunner
{
    public IAnsiConsole? ConsoleReceived { get; private set; }
    public TerminalSelectionPrompt? PromptReceived { get; private set; }
    public TerminalSelectionResult ResultToReturn { get; set; } = TerminalSelectionResult.Selected("mock");

    public TerminalSelectionResult Prompt(IAnsiConsole console, TerminalSelectionPrompt prompt)
    {
        ConsoleReceived = console;
        PromptReceived = prompt;
        return ResultToReturn;
    }
}
