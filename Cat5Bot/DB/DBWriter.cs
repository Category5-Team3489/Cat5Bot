namespace Cat5Bot.DB; //{}

#nullable disable

// Adapted from: https://github.com/RevenantX/LiteNetLib/blob/master/LiteNetLib/Utils/NetDataWriter.cs
public class DBWriter
{
    protected byte[] _data;
    protected int _position;
    private const int InitialSize = 64;
    private readonly bool _autoResize;

    public int Capacity => _data.Length;
    public byte[] Data => _data;
    public int Length => _position;

    public DBWriter() : this(true, InitialSize) {}

    public DBWriter(bool autoResize) : this(autoResize, InitialSize) {}

    public DBWriter(bool autoResize, int initialSize)
    {
        _data = new byte[initialSize];
        _autoResize = autoResize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResizeIfNeed(int newSize)
    {
        if (_data.Length < newSize)
        {
            Resize(newSize);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Resize(int newSize)
    {
        int len = _data.Length;
        while (len < newSize)
            len *= 2;
        Array.Resize(ref _data, len);
    }

    public void Reset(int size)
    {
        ResizeIfNeed(size);
        _position = 0;
    }

    public void Reset()
    {
        _position = 0;
    }

    public byte[] CopyData()
    {
        byte[] resultData = new byte[_position];
        Buffer.BlockCopy(_data, 0, resultData, 0, _position);
        return resultData;
    }

    public void Put(float value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 4);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 4;
    }

    public void Put(double value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 8);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 8;
    }

    public void Put(long value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 8);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 8;
    }

    public void Put(ulong value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 8);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 8;
    }

    public void Put(int value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 4);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 4;
    }

    public void Put(uint value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 4);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 4;
    }

    public void Put(char value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 2);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 2;
    }

    public void Put(ushort value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 2);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 2;
    }

    public void Put(short value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 2);
        FastBitConverter.GetBytes(_data, _position, value);
        _position += 2;
    }

    public void Put(sbyte value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 1);
        _data[_position] = (byte)value;
        _position++;
    }

    public void Put(byte value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 1);
        _data[_position] = value;
        _position++;
    }

    public void Put(byte[] data, int offset, int length)
    {
        if (_autoResize)
            ResizeIfNeed(_position + length);
        Buffer.BlockCopy(data, offset, _data, _position, length);
        _position += length;
    }

    public void Put(byte[] data)
    {
        if (_autoResize)
            ResizeIfNeed(_position + data.Length);
        Buffer.BlockCopy(data, 0, _data, _position, data.Length);
        _position += data.Length;
    }

    public void PutSBytesWithLength(sbyte[] data, int offset, int length)
    {
        if (_autoResize)
            ResizeIfNeed(_position + length + 4);
        FastBitConverter.GetBytes(_data, _position, length);
        Buffer.BlockCopy(data, offset, _data, _position + 4, length);
        _position += length + 4;
    }

    public void PutSBytesWithLength(sbyte[] data)
    {
        if (_autoResize)
            ResizeIfNeed(_position + data.Length + 4);
        FastBitConverter.GetBytes(_data, _position, data.Length);
        Buffer.BlockCopy(data, 0, _data, _position + 4, data.Length);
        _position += data.Length + 4;
    }

    public void PutBytesWithLength(byte[] data, int offset, int length)
    {
        if (_autoResize)
            ResizeIfNeed(_position + length + 4);
        FastBitConverter.GetBytes(_data, _position, length);
        Buffer.BlockCopy(data, offset, _data, _position + 4, length);
        _position += length + 4;
    }

    public void PutBytesWithLength(byte[] data)
    {
        if (_autoResize)
            ResizeIfNeed(_position + data.Length + 4);
        FastBitConverter.GetBytes(_data, _position, data.Length);
        Buffer.BlockCopy(data, 0, _data, _position + 4, data.Length);
        _position += data.Length + 4;
    }

    public void Put(bool value)
    {
        if (_autoResize)
            ResizeIfNeed(_position + 1);
        _data[_position] = (byte)(value ? 1 : 0);
        _position++;
    }

    private void PutArray(Array arr, int sz)
    {
        ushort length = arr == null ? (ushort) 0 : (ushort)arr.Length;
        sz *= length;
        if (_autoResize)
            ResizeIfNeed(_position + sz + 2);
        FastBitConverter.GetBytes(_data, _position, length);
        if (arr != null)
            Buffer.BlockCopy(arr, 0, _data, _position + 2, sz);
        _position += sz + 2;
    }

    public void PutArray(float[] value)
    {
        PutArray(value, 4);
    }

    public void PutArray(double[] value)
    {
        PutArray(value, 8);
    }

    public void PutArray(long[] value)
    {
        PutArray(value, 8);
    }

    public void PutArray(ulong[] value)
    {
        PutArray(value, 8);
    }

    public void PutArray(int[] value)
    {
        PutArray(value, 4);
    }

    public void PutArray(uint[] value)
    {
        PutArray(value, 4);
    }

    public void PutArray(ushort[] value)
    {
        PutArray(value, 2);
    }

    public void PutArray(short[] value)
    {
        PutArray(value, 2);
    }

    public void PutArray(bool[] value)
    {
        PutArray(value, 1);
    }

    public void PutArray(string[] value)
    {
        ushort len = value == null ? (ushort)0 : (ushort)value.Length;
        Put(len);
        for (int i = 0; i < len; i++)
            Put(value[i]);
    }

    public void PutArray(string[] value, int maxLength)
    {
        ushort len = value == null ? (ushort)0 : (ushort)value.Length;
        Put(len);
        for (int i = 0; i < len; i++)
            Put(value[i], maxLength);
    }

    private void PutArrayLong(Array arr, int sz)
    {
        int length = arr == null ? 0 : arr.Length;
        sz *= length;
        if (_autoResize)
            ResizeIfNeed(_position + sz + 4);
        FastBitConverter.GetBytes(_data, _position, length);
        if (arr != null)
            Buffer.BlockCopy(arr, 0, _data, _position + 4, sz);
        _position += sz + 4;
    }

    public void PutArrayLong(float[] value)
    {
        PutArrayLong(value, 4);
    }

    public void PutArrayLong(double[] value)
    {
        PutArrayLong(value, 8);
    }

    public void PutArrayLong(long[] value)
    {
        PutArrayLong(value, 8);
    }

    public void PutArrayLong(ulong[] value)
    {
        PutArrayLong(value, 8);
    }

    public void PutArrayLong(int[] value)
    {
        PutArrayLong(value, 4);
    }

    public void PutArrayLong(uint[] value)
    {
        PutArrayLong(value, 4);
    }

    public void PutArrayLong(ushort[] value)
    {
        PutArrayLong(value, 2);
    }

    public void PutArrayLong(short[] value)
    {
        PutArrayLong(value, 2);
    }

    public void PutArrayLong(bool[] value)
    {
        PutArrayLong(value, 1);
    }

    public void PutArrayLong(string[] value)
    {
        int len = value == null ? 0 : value.Length;
        Put(len);
        for (int i = 0; i < len; i++)
            Put(value[i]);
    }

    public void PutArrayLong(string[] value, int maxLength)
    {
        int len = value == null ? 0 : value.Length;
        Put(len);
        for (int i = 0; i < len; i++)
            Put(value[i], maxLength);
    }

    public void Put(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Put(0);
            return;
        }

        //put bytes count
        int bytesCount = Encoding.UTF8.GetByteCount(value);
        if (_autoResize)
            ResizeIfNeed(_position + bytesCount + 4);
        Put(bytesCount);

        //put string
        Encoding.UTF8.GetBytes(value, 0, value.Length, _data, _position);
        _position += bytesCount;
    }

    public void Put(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            Put(0);
            return;
        }

        int length = value.Length > maxLength ? maxLength : value.Length;

        int totalBytesCount = Encoding.UTF8.GetMaxByteCount(length); //gets max length irrespective of actual length

        if (_autoResize)
            ResizeIfNeed(_position + totalBytesCount + 4);

        int countPosition = _position; //save position where length needs to be stored
        _position += 4;

        int requiredBytesCount = Encoding.UTF8.GetBytes(value, 0, length, _data, _position); //put string here
        int positionAfterWrite = _position + requiredBytesCount; //position where string data ends

        _position = countPosition; //go to position where we need to write int value

        Put(requiredBytesCount); //put length of substring
        _position = positionAfterWrite; //reset position to final position
    }
}