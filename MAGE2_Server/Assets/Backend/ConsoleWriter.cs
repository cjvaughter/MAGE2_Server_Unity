using System.IO;
using System.Text;
using UnityEngine.UI;

public class ConsoleWriter : TextWriter
{
    private Text _console;

    public ConsoleWriter(Text text)
    {
        _console = text;
    }

    public override void Write(char value)
    {
        _console.text += value;
    }

    public override void Write(string value)
    {
        _console.text += value;
    }

    public override void WriteLine(char value)
    {
        Write(value + "\r\n");
    }

    public override void WriteLine(string value)
    {
        Write(value + "\r\n");
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
}
