using System;
using System.IO;

namespace Coosu.Shared.IO;

public class EphemeralLineReader : IDisposable
{
    private readonly TextReader _reader;
    private char[] _lineBuffer;
    private const int InitialLineBufferSize = 256; // 初始缓冲区大小，可以调整

    public EphemeralLineReader(TextReader reader, int initialCapacity = InitialLineBufferSize)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        if (initialCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Capacity must be positive.");
        _lineBuffer = new char[initialCapacity];
    }

    /// <summary>
    /// 从文本读取器中读取一行字符，并将数据作为 ReadOnlyMemory&lt;char&gt; 返回。
    /// 返回的内存仅在下一次调用 ReadLine 之前有效。
    /// </summary>
    /// <returns>一个包含来自文本读取器的下一行的 ReadOnlyMemory&lt;char&gt;；如果已到达读取器的末尾，则为 null。</returns>
    public ReadOnlyMemory<char>? ReadLine()
    {
        int pos = 0;
        int chValue;

        while (true)
        {
            chValue = _reader.Read();
            if (chValue == -1) // EOF
            {
                break;
            }

            char ch = (char)chValue;
            if (ch == '\n') // LF
            {
                break;
            }
            if (ch == '\r') // CR
            {
                // 检查 CRLF
                if (_reader.Peek() == '\n')
                {
                    _reader.Read(); // 消耗 LF
                }
                break;
            }

            if (pos >= _lineBuffer.Length)
            {
                // 增长缓冲区。对于高频/长行，可以考虑 ArrayPool。
                // 为简单起见，使用 Array.Resize。
                int newSize = _lineBuffer.Length * 2;
                // 防止极端增长的基本保护，尽管 TextReader 本身可能会缓冲。
                if (newSize > 1_048_576 && _lineBuffer.Length > 1_048_576) // 1MB
                {
                    newSize = _lineBuffer.Length + 1_048_576; // 对于非常大的行，适度限制增长
                }
                else if (newSize == 0 && _lineBuffer.Length == 0) // 如果初始大小为0或缓冲区长度变为0
                {
                    newSize = InitialLineBufferSize; // 重置为初始大小
                }
                else if (newSize < _lineBuffer.Length) // 溢出检查
                {
                    newSize = int.MaxValue;
                }

                if (newSize == 0 && InitialLineBufferSize > 0) newSize = InitialLineBufferSize; // 确保不会分配0长度数组
                if (newSize <= pos) newSize = pos + InitialLineBufferSize; // 确保新大小至少能容纳当前内容+一些余量

                try
                {
                    Array.Resize(ref _lineBuffer, newSize);
                }
                catch (OutOfMemoryException)
                {
                    // 如果无法分配更大的缓冲区，则处理此情况
                    // 例如，可以抛出自定义异常或返回已读取的部分
                    // 为了简单起见，我们在这里让异常传播
                    throw;
                }
            }
            _lineBuffer[pos++] = ch;
        }

        if (chValue == -1 && pos == 0) // EOF 并且没有为该行读取任何字符
        {
            return null;
        }

        return _lineBuffer.AsMemory(0, pos);
    }

    public void Dispose()
    {
        _reader.Dispose();
        // 如果使用 ArrayPool，在此处返回缓冲区。
    }
}