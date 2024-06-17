// Decompiled with JetBrains decompiler
// Type: BigInteger
// Assembly: MegaApiClient, Version=1.10.3.0, Culture=neutral, PublicKeyToken=0480d311efbeb4e2
// MVID: FED3AEAC-D757-4BF7-8E81-293AB6C488BC
// Assembly location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.dll
// XML documentation location: D:\Old C# Projets\MegaChecker\MegaChecker\bin\Debug\MegaApiClient.xml

using System;
using System.Collections.Generic;

#nullable disable
/// <summary>
/// This is a BigInteger class. Holds integer that is more than 64-bit (long).
/// </summary>
/// <remarks>
/// This class contains overloaded arithmetic operators(+, -, *, /, %), bitwise operators(&amp;, |) and other
/// operations that can be done with normal integer. It also contains implementation of various prime test.
/// This class also contains methods dealing with cryptography such as generating prime number, generating
/// a coprime number.
/// </remarks>
public class BigInteger
{
  private const int maxLength = 1000;
  public static readonly int[] primesBelow2000 = new int[303]
  {
    2,
    3,
    5,
    7,
    11,
    13,
    17,
    19,
    23,
    29,
    31,
    37,
    41,
    43,
    47,
    53,
    59,
    61,
    67,
    71,
    73,
    79,
    83,
    89,
    97,
    101,
    103,
    107,
    109,
    113,
    (int) sbyte.MaxValue,
    131,
    137,
    139,
    149,
    151,
    157,
    163,
    167,
    173,
    179,
    181,
    191,
    193,
    197,
    199,
    211,
    223,
    227,
    229,
    233,
    239,
    241,
    251,
    257,
    263,
    269,
    271,
    277,
    281,
    283,
    293,
    307,
    311,
    313,
    317,
    331,
    337,
    347,
    349,
    353,
    359,
    367,
    373,
    379,
    383,
    389,
    397,
    401,
    409,
    419,
    421,
    431,
    433,
    439,
    443,
    449,
    457,
    461,
    463,
    467,
    479,
    487,
    491,
    499,
    503,
    509,
    521,
    523,
    541,
    547,
    557,
    563,
    569,
    571,
    577,
    587,
    593,
    599,
    601,
    607,
    613,
    617,
    619,
    631,
    641,
    643,
    647,
    653,
    659,
    661,
    673,
    677,
    683,
    691,
    701,
    709,
    719,
    727,
    733,
    739,
    743,
    751,
    757,
    761,
    769,
    773,
    787,
    797,
    809,
    811,
    821,
    823,
    827,
    829,
    839,
    853,
    857,
    859,
    863,
    877,
    881,
    883,
    887,
    907,
    911,
    919,
    929,
    937,
    941,
    947,
    953,
    967,
    971,
    977,
    983,
    991,
    997,
    1009,
    1013,
    1019,
    1021,
    1031,
    1033,
    1039,
    1049,
    1051,
    1061,
    1063,
    1069,
    1087,
    1091,
    1093,
    1097,
    1103,
    1109,
    1117,
    1123,
    1129,
    1151,
    1153,
    1163,
    1171,
    1181,
    1187,
    1193,
    1201,
    1213,
    1217,
    1223,
    1229,
    1231,
    1237,
    1249,
    1259,
    1277,
    1279,
    1283,
    1289,
    1291,
    1297,
    1301,
    1303,
    1307,
    1319,
    1321,
    1327,
    1361,
    1367,
    1373,
    1381,
    1399,
    1409,
    1423,
    1427,
    1429,
    1433,
    1439,
    1447,
    1451,
    1453,
    1459,
    1471,
    1481,
    1483,
    1487,
    1489,
    1493,
    1499,
    1511,
    1523,
    1531,
    1543,
    1549,
    1553,
    1559,
    1567,
    1571,
    1579,
    1583,
    1597,
    1601,
    1607,
    1609,
    1613,
    1619,
    1621,
    1627,
    1637,
    1657,
    1663,
    1667,
    1669,
    1693,
    1697,
    1699,
    1709,
    1721,
    1723,
    1733,
    1741,
    1747,
    1753,
    1759,
    1777,
    1783,
    1787,
    1789,
    1801,
    1811,
    1823,
    1831,
    1847,
    1861,
    1867,
    1871,
    1873,
    1877,
    1879,
    1889,
    1901,
    1907,
    1913,
    1931,
    1933,
    1949,
    1951,
    1973,
    1979,
    1987,
    1993,
    1997,
    1999
  };
  private uint[] data;
  public int dataLength;

  /// <summary>Default constructor for BigInteger of value 0</summary>
  public BigInteger()
  {
    this.data = new uint[1000];
    this.dataLength = 1;
  }

  /// <summary>Constructor (Default value provided by long)</summary>
  /// <param name="value">Turn the long value into BigInteger type</param>
  public BigInteger(long value)
  {
    this.data = new uint[1000];
    long num = value;
    for (this.dataLength = 0; value != 0L && this.dataLength < 1000; ++this.dataLength)
    {
      this.data[this.dataLength] = (uint) ((ulong) value & (ulong) uint.MaxValue);
      value >>= 32;
    }
    if (num > 0L)
    {
      if (value != 0L || ((int) this.data[999] & int.MinValue) != 0)
        throw new ArithmeticException("Positive overflow in constructor.");
    }
    else if (num < 0L && (value != -1L || ((int) this.data[this.dataLength - 1] & int.MinValue) == 0))
      throw new ArithmeticException("Negative underflow in constructor.");
    if (this.dataLength != 0)
      return;
    this.dataLength = 1;
  }

  /// <summary>Constructor (Default value provided by ulong)</summary>
  /// <param name="value">Turn the unsigned long value into BigInteger type</param>
  public BigInteger(ulong value)
  {
    this.data = new uint[1000];
    for (this.dataLength = 0; value != 0UL && this.dataLength < 1000; ++this.dataLength)
    {
      this.data[this.dataLength] = (uint) (value & (ulong) uint.MaxValue);
      value >>= 32;
    }
    if (value != 0UL || ((int) this.data[999] & int.MinValue) != 0)
      throw new ArithmeticException("Positive overflow in constructor.");
    if (this.dataLength != 0)
      return;
    this.dataLength = 1;
  }

  /// <summary>Constructor (Default value provided by BigInteger)</summary>
  /// <param name="bi"></param>
  public BigInteger(BigInteger bi)
  {
    this.data = new uint[1000];
    this.dataLength = bi.dataLength;
    for (int index = 0; index < this.dataLength; ++index)
      this.data[index] = bi.data[index];
  }

  /// <summary>
  /// Constructor (Default value provided by a string of digits of the specified base)
  /// </summary>
  /// <example>
  /// To initialize "a" with the default value of 1234 in base 10:
  ///      BigInteger a = new BigInteger("1234", 10)
  /// To initialize "a" with the default value of -0x1D4F in base 16:
  ///      BigInteger a = new BigInteger("-1D4F", 16)
  /// </example>
  /// <param name="value">String value in the format of [sign][magnitude]</param>
  /// <param name="radix">The base of value</param>
  public BigInteger(string value, int radix)
  {
    BigInteger bigInteger1 = new BigInteger(1L);
    BigInteger bigInteger2 = new BigInteger();
    value = value.ToUpper().Trim();
    int num1 = 0;
    if (value[0] == '-')
      num1 = 1;
    for (int index = value.Length - 1; index >= num1; --index)
    {
      int num2 = (int) value[index];
      int num3 = num2 < 48 || num2 > 57 ? (num2 < 65 || num2 > 90 ? 9999999 : num2 - 65 + 10) : num2 - 48;
      if (num3 >= radix)
        throw new ArithmeticException("Invalid string in constructor.");
      if (value[0] == '-')
        num3 = -num3;
      bigInteger2 += bigInteger1 * (BigInteger) num3;
      if (index - 1 >= num1)
        bigInteger1 *= (BigInteger) radix;
    }
    if (value[0] == '-')
    {
      if (((int) bigInteger2.data[999] & int.MinValue) == 0)
        throw new ArithmeticException("Negative underflow in constructor.");
    }
    else if (((int) bigInteger2.data[999] & int.MinValue) != 0)
      throw new ArithmeticException("Positive overflow in constructor.");
    this.data = new uint[1000];
    for (int index = 0; index < bigInteger2.dataLength; ++index)
      this.data[index] = bigInteger2.data[index];
    this.dataLength = bigInteger2.dataLength;
  }

  /// <summary>
  /// Constructor (Default value provided by an array of bytes of the specified length.)
  /// </summary>
  /// <param name="inData">A list of byte values</param>
  /// <param name="length">Default -1</param>
  /// <param name="offset">Default 0</param>
  public BigInteger(IList<byte> inData, int length = -1, int offset = 0)
  {
    int num1 = length == -1 ? inData.Count - offset : length;
    this.dataLength = num1 >> 2;
    int num2 = num1 & 3;
    if (num2 != 0)
      ++this.dataLength;
    if (this.dataLength > 1000 || num1 > inData.Count - offset)
      throw new ArithmeticException("Byte overflow in constructor.");
    this.data = new uint[1000];
    int num3 = num1 - 1;
    int index = 0;
    while (num3 >= 3)
    {
      this.data[index] = (uint) (((int) inData[offset + num3 - 3] << 24) + ((int) inData[offset + num3 - 2] << 16) + ((int) inData[offset + num3 - 1] << 8)) + (uint) inData[offset + num3];
      num3 -= 4;
      ++index;
    }
    switch (num2)
    {
      case 1:
        this.data[this.dataLength - 1] = (uint) inData[offset];
        break;
      case 2:
        this.data[this.dataLength - 1] = ((uint) inData[offset] << 8) + (uint) inData[offset + 1];
        break;
      case 3:
        this.data[this.dataLength - 1] = (uint) (((int) inData[offset] << 16) + ((int) inData[offset + 1] << 8)) + (uint) inData[offset + 2];
        break;
    }
    if (this.dataLength == 0)
      this.dataLength = 1;
    while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
      --this.dataLength;
  }

  /// <summary>
  ///  Constructor (Default value provided by an array of unsigned integers)
  /// </summary>
  /// <param name="inData">Array of unsigned integer</param>
  public BigInteger(uint[] inData)
  {
    this.dataLength = inData.Length;
    if (this.dataLength > 1000)
      throw new ArithmeticException("Byte overflow in constructor.");
    this.data = new uint[1000];
    int index1 = this.dataLength - 1;
    int index2 = 0;
    while (index1 >= 0)
    {
      this.data[index2] = inData[index1];
      --index1;
      ++index2;
    }
    while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
      --this.dataLength;
  }

  /// <summary>Cast a type long value to type BigInteger value</summary>
  /// <param name="value">A long value</param>
  public static implicit operator BigInteger(long value) => new BigInteger(value);

  /// <summary>Cast a type ulong value to type BigInteger value</summary>
  /// <param name="value">An unsigned long value</param>
  public static implicit operator BigInteger(ulong value) => new BigInteger(value);

  /// <summary>Cast a type int value to type BigInteger value</summary>
  /// <param name="value">An int value</param>
  public static implicit operator BigInteger(int value) => new BigInteger((long) value);

  /// <summary>Cast a type uint value to type BigInteger value</summary>
  /// <param name="value">An unsigned int value</param>
  public static implicit operator BigInteger(uint value) => new BigInteger((ulong) value);

  /// <summary>Overloading of addition operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Result of the addition of 2 BigIntegers</returns>
  public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
  {
    BigInteger bigInteger = new BigInteger()
    {
      dataLength = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength
    };
    long num1 = 0;
    for (int index = 0; index < bigInteger.dataLength; ++index)
    {
      long num2 = (long) bi1.data[index] + (long) bi2.data[index] + num1;
      num1 = num2 >> 32;
      bigInteger.data[index] = (uint) ((ulong) num2 & (ulong) uint.MaxValue);
    }
    if (num1 != 0L && bigInteger.dataLength < 1000)
    {
      bigInteger.data[bigInteger.dataLength] = (uint) num1;
      ++bigInteger.dataLength;
    }
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    int index1 = 999;
    if (((int) bi1.data[index1] & int.MinValue) == ((int) bi2.data[index1] & int.MinValue) && ((int) bigInteger.data[index1] & int.MinValue) != ((int) bi1.data[index1] & int.MinValue))
      throw new ArithmeticException();
    return bigInteger;
  }

  /// <summary>
  /// Overloading of the unary ++ operator, which increments BigInteger by 1
  /// </summary>
  /// <param name="bi1">A BigInteger</param>
  /// <returns>Incremented BigInteger</returns>
  public static BigInteger operator ++(BigInteger bi1)
  {
    BigInteger bigInteger = new BigInteger(bi1);
    long num1 = 1;
    int index1;
    for (index1 = 0; num1 != 0L && index1 < 1000; ++index1)
    {
      long num2 = (long) bigInteger.data[index1] + 1L;
      bigInteger.data[index1] = (uint) ((ulong) num2 & (ulong) uint.MaxValue);
      num1 = num2 >> 32;
    }
    if (index1 > bigInteger.dataLength)
    {
      bigInteger.dataLength = index1;
    }
    else
    {
      while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
        --bigInteger.dataLength;
    }
    int index2 = 999;
    if (((int) bi1.data[index2] & int.MinValue) == 0 && ((int) bigInteger.data[index2] & int.MinValue) != ((int) bi1.data[index2] & int.MinValue))
      throw new ArithmeticException("Overflow in ++.");
    return bigInteger;
  }

  /// <summary>Overloading of subtraction operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Result of the subtraction of 2 BigIntegers</returns>
  public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
  {
    BigInteger bigInteger = new BigInteger()
    {
      dataLength = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength
    };
    long num1 = 0;
    for (int index = 0; index < bigInteger.dataLength; ++index)
    {
      long num2 = (long) bi1.data[index] - (long) bi2.data[index] - num1;
      bigInteger.data[index] = (uint) ((ulong) num2 & (ulong) uint.MaxValue);
      num1 = num2 >= 0L ? 0L : 1L;
    }
    if (num1 != 0L)
    {
      for (int dataLength = bigInteger.dataLength; dataLength < 1000; ++dataLength)
        bigInteger.data[dataLength] = uint.MaxValue;
      bigInteger.dataLength = 1000;
    }
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    int index1 = 999;
    if (((int) bi1.data[index1] & int.MinValue) != ((int) bi2.data[index1] & int.MinValue) && ((int) bigInteger.data[index1] & int.MinValue) != ((int) bi1.data[index1] & int.MinValue))
      throw new ArithmeticException();
    return bigInteger;
  }

  /// <summary>
  /// Overloading of the unary -- operator, decrements BigInteger by 1
  /// </summary>
  /// <param name="bi1">A BigInteger</param>
  /// <returns>Decremented BigInteger</returns>
  public static BigInteger operator --(BigInteger bi1)
  {
    BigInteger bigInteger = new BigInteger(bi1);
    bool flag = true;
    int index1;
    for (index1 = 0; flag && index1 < 1000; ++index1)
    {
      long num = (long) bigInteger.data[index1] - 1L;
      bigInteger.data[index1] = (uint) ((ulong) num & (ulong) uint.MaxValue);
      if (num >= 0L)
        flag = false;
    }
    if (index1 > bigInteger.dataLength)
      bigInteger.dataLength = index1;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    int index2 = 999;
    if (((int) bi1.data[index2] & int.MinValue) != 0 && ((int) bigInteger.data[index2] & int.MinValue) != ((int) bi1.data[index2] & int.MinValue))
      throw new ArithmeticException("Underflow in --.");
    return bigInteger;
  }

  /// <summary>Overloading of multiplication operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Result of the multiplication of 2 BigIntegers</returns>
  public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
  {
    int index1 = 999;
    bool flag1 = false;
    bool flag2 = false;
    try
    {
      if (((int) bi1.data[index1] & int.MinValue) != 0)
      {
        flag1 = true;
        bi1 = -bi1;
      }
      if (((int) bi2.data[index1] & int.MinValue) != 0)
      {
        flag2 = true;
        bi2 = -bi2;
      }
    }
    catch (Exception ex)
    {
    }
    BigInteger bigInteger = new BigInteger();
    try
    {
      for (int index2 = 0; index2 < bi1.dataLength; ++index2)
      {
        if (bi1.data[index2] != 0U)
        {
          ulong num1 = 0;
          int index3 = 0;
          int index4 = index2;
          while (index3 < bi2.dataLength)
          {
            ulong num2 = (ulong) bi1.data[index2] * (ulong) bi2.data[index3] + (ulong) bigInteger.data[index4] + num1;
            bigInteger.data[index4] = (uint) (num2 & (ulong) uint.MaxValue);
            num1 = num2 >> 32;
            ++index3;
            ++index4;
          }
          if (num1 != 0UL)
            bigInteger.data[index2 + bi2.dataLength] = (uint) num1;
        }
      }
    }
    catch (Exception ex)
    {
      throw new ArithmeticException("Multiplication overflow.");
    }
    bigInteger.dataLength = bi1.dataLength + bi2.dataLength;
    if (bigInteger.dataLength > 1000)
      bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    if (((int) bigInteger.data[index1] & int.MinValue) != 0)
    {
      if (flag1 != flag2 && bigInteger.data[index1] == 2147483648U)
      {
        if (bigInteger.dataLength == 1)
          return bigInteger;
        bool flag3 = true;
        for (int index5 = 0; index5 < bigInteger.dataLength - 1 & flag3; ++index5)
        {
          if (bigInteger.data[index5] != 0U)
            flag3 = false;
        }
        if (flag3)
          return bigInteger;
      }
      throw new ArithmeticException("Multiplication overflow.");
    }
    return flag1 != flag2 ? -bigInteger : bigInteger;
  }

  /// <summary>
  /// Overloading of the unary &lt;&lt; operator (left shift)
  /// </summary>
  /// <remarks>
  /// Shifting by a negative number is an undefined behaviour (UB).
  /// </remarks>
  /// <param name="bi1">A BigInteger</param>
  /// <param name="shiftVal">Left shift by shiftVal bit</param>
  /// <returns>Left-shifted BigInteger</returns>
  public static BigInteger operator <<(BigInteger bi1, int shiftVal)
  {
    BigInteger bigInteger = new BigInteger(bi1);
    bigInteger.dataLength = BigInteger.shiftLeft(bigInteger.data, shiftVal);
    return bigInteger;
  }

  private static int shiftLeft(uint[] buffer, int shiftVal)
  {
    int num1 = 32;
    int length = buffer.Length;
    while (length > 1 && buffer[length - 1] == 0U)
      --length;
    for (int index1 = shiftVal; index1 > 0; index1 -= num1)
    {
      if (index1 < num1)
        num1 = index1;
      ulong num2 = 0;
      for (int index2 = 0; index2 < length; ++index2)
      {
        ulong num3 = (ulong) buffer[index2] << num1 | num2;
        buffer[index2] = (uint) (num3 & (ulong) uint.MaxValue);
        num2 = num3 >> 32;
      }
      if (num2 != 0UL && length + 1 <= buffer.Length)
      {
        buffer[length] = (uint) num2;
        ++length;
      }
    }
    return length;
  }

  /// <summary>
  /// Overloading of the unary &gt;&gt; operator (right shift)
  /// </summary>
  /// <remarks>
  /// Shifting by a negative number is an undefined behaviour (UB).
  /// </remarks>
  /// <param name="bi1">A BigInteger</param>
  /// <param name="shiftVal">Right shift by shiftVal bit</param>
  /// <returns>Right-shifted BigInteger</returns>
  public static BigInteger operator >>(BigInteger bi1, int shiftVal)
  {
    BigInteger bigInteger = new BigInteger(bi1);
    bigInteger.dataLength = BigInteger.shiftRight(bigInteger.data, shiftVal);
    if (((int) bi1.data[999] & int.MinValue) != 0)
    {
      for (int index = 999; index >= bigInteger.dataLength; --index)
        bigInteger.data[index] = uint.MaxValue;
      uint num = 2147483648;
      for (int index = 0; index < 32 && ((int) bigInteger.data[bigInteger.dataLength - 1] & (int) num) == 0; ++index)
      {
        bigInteger.data[bigInteger.dataLength - 1] |= num;
        num >>= 1;
      }
      bigInteger.dataLength = 1000;
    }
    return bigInteger;
  }

  private static int shiftRight(uint[] buffer, int shiftVal)
  {
    int num1 = 32;
    int num2 = 0;
    int length = buffer.Length;
    while (length > 1 && buffer[length - 1] == 0U)
      --length;
    for (int index1 = shiftVal; index1 > 0; index1 -= num1)
    {
      if (index1 < num1)
      {
        num1 = index1;
        num2 = 32 - num1;
      }
      ulong num3 = 0;
      for (int index2 = length - 1; index2 >= 0; --index2)
      {
        ulong num4 = (ulong) buffer[index2] >> num1 | num3;
        num3 = (ulong) buffer[index2] << num2 & (ulong) uint.MaxValue;
        buffer[index2] = (uint) num4;
      }
    }
    while (length > 1 && buffer[length - 1] == 0U)
      --length;
    return length;
  }

  /// <summary>
  /// Overloading of the bit-wise NOT operator (1's complement)
  /// </summary>
  /// <param name="bi1">A BigInteger</param>
  /// <returns>Complemented BigInteger</returns>
  public static BigInteger operator ~(BigInteger bi1)
  {
    BigInteger bigInteger = new BigInteger(bi1);
    for (int index = 0; index < 1000; ++index)
      bigInteger.data[index] = ~bi1.data[index];
    bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    return bigInteger;
  }

  /// <summary>Overloading of the NEGATE operator (2's complement)</summary>
  /// <param name="bi1">A BigInteger</param>
  /// <returns>Negated BigInteger or default BigInteger value if bi1 is 0</returns>
  public static BigInteger operator -(BigInteger bi1)
  {
    if (bi1.dataLength == 1 && bi1.data[0] == 0U)
      return new BigInteger();
    BigInteger bigInteger = new BigInteger(bi1);
    for (int index = 0; index < 1000; ++index)
      bigInteger.data[index] = ~bi1.data[index];
    long num1 = 1;
    for (int index = 0; num1 != 0L && index < 1000; ++index)
    {
      long num2 = (long) bigInteger.data[index] + 1L;
      bigInteger.data[index] = (uint) ((ulong) num2 & (ulong) uint.MaxValue);
      num1 = num2 >> 32;
    }
    if (((int) bi1.data[999] & int.MinValue) == ((int) bigInteger.data[999] & int.MinValue))
      throw new ArithmeticException("Overflow in negation.\n");
    bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    return bigInteger;
  }

  /// <summary>
  /// Overloading of equality operator, allows comparing 2 BigIntegers with == operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator ==(BigInteger bi1, BigInteger bi2) => bi1.Equals((object) bi2);

  /// <summary>
  /// Overloading of not equal operator, allows comparing 2 BigIntegers with != operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator !=(BigInteger bi1, BigInteger bi2) => !bi1.Equals((object) bi2);

  /// <summary>
  /// Overriding of Equals method, allows comparing BigInteger with an arbitary object
  /// </summary>
  /// <param name="o">Input object, to be casted into BigInteger type for comparison</param>
  /// <returns>Boolean result of the comparison</returns>
  public override bool Equals(object o)
  {
    BigInteger bigInteger = (BigInteger) o;
    if (this.dataLength != bigInteger.dataLength)
      return false;
    for (int index = 0; index < this.dataLength; ++index)
    {
      if ((int) this.data[index] != (int) bigInteger.data[index])
        return false;
    }
    return true;
  }

  public override int GetHashCode() => this.ToString().GetHashCode();

  /// <summary>
  /// Overloading of greater than operator, allows comparing 2 BigIntegers with &gt; operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator >(BigInteger bi1, BigInteger bi2)
  {
    int index1 = 999;
    if (((int) bi1.data[index1] & int.MinValue) != 0 && ((int) bi2.data[index1] & int.MinValue) == 0)
      return false;
    if (((int) bi1.data[index1] & int.MinValue) == 0 && ((int) bi2.data[index1] & int.MinValue) != 0)
      return true;
    int index2 = (bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength) - 1;
    while (index2 >= 0 && (int) bi1.data[index2] == (int) bi2.data[index2])
      --index2;
    return index2 >= 0 && bi1.data[index2] > bi2.data[index2];
  }

  /// <summary>
  /// Overloading of greater than operator, allows comparing 2 BigIntegers with &lt; operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator <(BigInteger bi1, BigInteger bi2)
  {
    int index1 = 999;
    if (((int) bi1.data[index1] & int.MinValue) != 0 && ((int) bi2.data[index1] & int.MinValue) == 0)
      return true;
    if (((int) bi1.data[index1] & int.MinValue) == 0 && ((int) bi2.data[index1] & int.MinValue) != 0)
      return false;
    int index2 = (bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength) - 1;
    while (index2 >= 0 && (int) bi1.data[index2] == (int) bi2.data[index2])
      --index2;
    return index2 >= 0 && bi1.data[index2] < bi2.data[index2];
  }

  /// <summary>
  /// Overloading of greater than or equal to operator, allows comparing 2 BigIntegers with &gt;= operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator >=(BigInteger bi1, BigInteger bi2) => bi1 == bi2 || bi1 > bi2;

  /// <summary>
  /// Overloading of less than or equal to operator, allows comparing 2 BigIntegers with &lt;= operator
  /// </summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>Boolean result of the comparison</returns>
  public static bool operator <=(BigInteger bi1, BigInteger bi2) => bi1 == bi2 || bi1 < bi2;

  private static void multiByteDivide(
    BigInteger bi1,
    BigInteger bi2,
    BigInteger outQuotient,
    BigInteger outRemainder)
  {
    uint[] numArray = new uint[1000];
    int length1 = bi1.dataLength + 1;
    uint[] buffer = new uint[length1];
    uint num1 = 2147483648;
    uint num2 = bi2.data[bi2.dataLength - 1];
    int shiftVal = 0;
    int num3 = 0;
    for (; num1 != 0U && ((int) num2 & (int) num1) == 0; num1 >>= 1)
      ++shiftVal;
    for (int index = 0; index < bi1.dataLength; ++index)
      buffer[index] = bi1.data[index];
    BigInteger.shiftLeft(buffer, shiftVal);
    bi2 <<= shiftVal;
    int num4 = length1 - bi2.dataLength;
    int index1 = length1 - 1;
    ulong num5 = (ulong) bi2.data[bi2.dataLength - 1];
    ulong num6 = (ulong) bi2.data[bi2.dataLength - 2];
    int length2 = bi2.dataLength + 1;
    uint[] inData = new uint[length2];
    for (; num4 > 0; --num4)
    {
      long num7 = ((long) buffer[index1] << 32) + (long) buffer[index1 - 1];
      ulong num8 = (ulong) num7 / num5;
      ulong num9 = (ulong) num7 % num5;
      bool flag = false;
      while (!flag)
      {
        flag = true;
        if (num8 == 4294967296UL || num8 * num6 > (num9 << 32) + (ulong) buffer[index1 - 2])
        {
          --num8;
          num9 += num5;
          if (num9 < 4294967296UL)
            flag = false;
        }
      }
      for (int index2 = 0; index2 < length2; ++index2)
        inData[index2] = buffer[index1 - index2];
      BigInteger bigInteger1 = new BigInteger(inData);
      BigInteger bigInteger2 = bi2 * (BigInteger) (long) num8;
      while (bigInteger2 > bigInteger1)
      {
        --num8;
        bigInteger2 -= bi2;
      }
      BigInteger bigInteger3 = bigInteger1 - bigInteger2;
      for (int index3 = 0; index3 < length2; ++index3)
        buffer[index1 - index3] = bigInteger3.data[bi2.dataLength - index3];
      numArray[num3++] = (uint) num8;
      --index1;
    }
    outQuotient.dataLength = num3;
    int index4 = 0;
    int index5 = outQuotient.dataLength - 1;
    while (index5 >= 0)
    {
      outQuotient.data[index4] = numArray[index5];
      --index5;
      ++index4;
    }
    for (; index4 < 1000; ++index4)
      outQuotient.data[index4] = 0U;
    while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0U)
      --outQuotient.dataLength;
    if (outQuotient.dataLength == 0)
      outQuotient.dataLength = 1;
    outRemainder.dataLength = BigInteger.shiftRight(buffer, shiftVal);
    int index6;
    for (index6 = 0; index6 < outRemainder.dataLength; ++index6)
      outRemainder.data[index6] = buffer[index6];
    for (; index6 < 1000; ++index6)
      outRemainder.data[index6] = 0U;
  }

  private static void singleByteDivide(
    BigInteger bi1,
    BigInteger bi2,
    BigInteger outQuotient,
    BigInteger outRemainder)
  {
    uint[] numArray = new uint[1000];
    int num1 = 0;
    for (int index = 0; index < 1000; ++index)
      outRemainder.data[index] = bi1.data[index];
    outRemainder.dataLength = bi1.dataLength;
    while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0U)
      --outRemainder.dataLength;
    ulong num2 = (ulong) bi2.data[0];
    int index1 = outRemainder.dataLength - 1;
    ulong num3 = (ulong) outRemainder.data[index1];
    if (num3 >= num2)
    {
      ulong num4 = num3 / num2;
      numArray[num1++] = (uint) num4;
      outRemainder.data[index1] = (uint) (num3 % num2);
    }
    ulong num5;
    for (int index2 = index1 - 1; index2 >= 0; outRemainder.data[index2--] = (uint) (num5 % num2))
    {
      num5 = ((ulong) outRemainder.data[index2 + 1] << 32) + (ulong) outRemainder.data[index2];
      ulong num6 = num5 / num2;
      numArray[num1++] = (uint) num6;
      outRemainder.data[index2 + 1] = 0U;
    }
    outQuotient.dataLength = num1;
    int index3 = 0;
    int index4 = outQuotient.dataLength - 1;
    while (index4 >= 0)
    {
      outQuotient.data[index3] = numArray[index4];
      --index4;
      ++index3;
    }
    for (; index3 < 1000; ++index3)
      outQuotient.data[index3] = 0U;
    while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0U)
      --outQuotient.dataLength;
    if (outQuotient.dataLength == 0)
      outQuotient.dataLength = 1;
    while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0U)
      --outRemainder.dataLength;
  }

  /// <summary>Overloading of division operator</summary>
  /// <remarks>The dataLength of the divisor's absolute value must be less than maxLength</remarks>
  /// <param name="bi1">Dividend</param>
  /// <param name="bi2">Divisor</param>
  /// <returns>Quotient of the division</returns>
  public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
  {
    BigInteger outQuotient = new BigInteger();
    BigInteger outRemainder = new BigInteger();
    int index = 999;
    bool flag1 = false;
    bool flag2 = false;
    if (((int) bi1.data[index] & int.MinValue) != 0)
    {
      bi1 = -bi1;
      flag2 = true;
    }
    if (((int) bi2.data[index] & int.MinValue) != 0)
    {
      bi2 = -bi2;
      flag1 = true;
    }
    if (bi1 < bi2)
      return outQuotient;
    if (bi2.dataLength == 1)
      BigInteger.singleByteDivide(bi1, bi2, outQuotient, outRemainder);
    else
      BigInteger.multiByteDivide(bi1, bi2, outQuotient, outRemainder);
    return flag2 != flag1 ? -outQuotient : outQuotient;
  }

  /// <summary>Overloading of modulus operator</summary>
  /// <remarks>The dataLength of the divisor's absolute value must be less than maxLength</remarks>
  /// <param name="bi1">Dividend</param>
  /// <param name="bi2">Divisor</param>
  /// <returns>Remainder of the division</returns>
  public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
  {
    BigInteger outQuotient = new BigInteger();
    BigInteger outRemainder = new BigInteger(bi1);
    int index = 999;
    bool flag = false;
    if (((int) bi1.data[index] & int.MinValue) != 0)
    {
      bi1 = -bi1;
      flag = true;
    }
    if (((int) bi2.data[index] & int.MinValue) != 0)
      bi2 = -bi2;
    if (bi1 < bi2)
      return outRemainder;
    if (bi2.dataLength == 1)
      BigInteger.singleByteDivide(bi1, bi2, outQuotient, outRemainder);
    else
      BigInteger.multiByteDivide(bi1, bi2, outQuotient, outRemainder);
    return flag ? -outRemainder : outRemainder;
  }

  /// <summary>Overloading of bitwise AND operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>BigInteger result after performing &amp; operation</returns>
  public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
  {
    BigInteger bigInteger = new BigInteger();
    int num1 = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;
    for (int index = 0; index < num1; ++index)
    {
      uint num2 = bi1.data[index] & bi2.data[index];
      bigInteger.data[index] = num2;
    }
    bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    return bigInteger;
  }

  /// <summary>Overloading of bitwise OR operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>BigInteger result after performing | operation</returns>
  public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
  {
    BigInteger bigInteger = new BigInteger();
    int num1 = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;
    for (int index = 0; index < num1; ++index)
    {
      uint num2 = bi1.data[index] | bi2.data[index];
      bigInteger.data[index] = num2;
    }
    bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    return bigInteger;
  }

  /// <summary>Overloading of bitwise XOR operator</summary>
  /// <param name="bi1">First BigInteger</param>
  /// <param name="bi2">Second BigInteger</param>
  /// <returns>BigInteger result after performing ^ operation</returns>
  public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
  {
    BigInteger bigInteger = new BigInteger();
    int num1 = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;
    for (int index = 0; index < num1; ++index)
    {
      uint num2 = bi1.data[index] ^ bi2.data[index];
      bigInteger.data[index] = num2;
    }
    bigInteger.dataLength = 1000;
    while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
      --bigInteger.dataLength;
    return bigInteger;
  }

  /// <summary>
  /// Compare this and a BigInteger and find the maximum one
  /// </summary>
  /// <param name="bi">BigInteger to be compared with this</param>
  /// <returns>The bigger value of this and bi</returns>
  public BigInteger max(BigInteger bi) => this > bi ? new BigInteger(this) : new BigInteger(bi);

  /// <summary>
  /// Compare this and a BigInteger and find the minimum one
  /// </summary>
  /// <param name="bi">BigInteger to be compared with this</param>
  /// <returns>The smaller value of this and bi</returns>
  public BigInteger min(BigInteger bi) => this < bi ? new BigInteger(this) : new BigInteger(bi);

  /// <summary>Returns the absolute value</summary>
  /// <returns>Absolute value of this</returns>
  public BigInteger abs()
  {
    return ((int) this.data[999] & int.MinValue) != 0 ? -this : new BigInteger(this);
  }

  /// <summary>
  /// Returns a string representing the BigInteger in base 10
  /// </summary>
  /// <returns>string representation of the BigInteger</returns>
  public override string ToString() => this.ToString(10);

  /// <summary>
  /// Returns a string representing the BigInteger in [sign][magnitude] format in the specified radix
  /// </summary>
  /// <example>If the value of BigInteger is -255 in base 10, then ToString(16) returns "-FF"</example>
  /// <param name="radix">Base</param>
  /// <returns>string representation of the BigInteger in [sign][magnitude] format</returns>
  public string ToString(int radix)
  {
    if (radix < 2 || radix > 36)
      throw new ArgumentException("Radix must be >= 2 and <= 36");
    string str1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string str2 = "";
    BigInteger bi1 = this;
    bool flag = false;
    if (((int) bi1.data[999] & int.MinValue) != 0)
    {
      flag = true;
      try
      {
        bi1 = -bi1;
      }
      catch (Exception ex)
      {
      }
    }
    BigInteger outQuotient = new BigInteger();
    BigInteger outRemainder = new BigInteger();
    BigInteger bi2 = new BigInteger((long) radix);
    if (bi1.dataLength == 1 && bi1.data[0] == 0U)
    {
      str2 = "0";
    }
    else
    {
      for (; bi1.dataLength > 1 || bi1.dataLength == 1 && bi1.data[0] != 0U; bi1 = outQuotient)
      {
        BigInteger.singleByteDivide(bi1, bi2, outQuotient, outRemainder);
        str2 = outRemainder.data[0] >= 10U ? str1[(int) outRemainder.data[0] - 10].ToString() + str2 : outRemainder.data[0].ToString() + str2;
      }
      if (flag)
        str2 = "-" + str2;
    }
    return str2;
  }

  /// <summary>
  /// Returns a hex string showing the contains of the BigInteger
  /// </summary>
  /// <example>
  /// 1) If the value of BigInteger is 255 in base 10, then ToHexString() returns "FF"
  /// 2) If the value of BigInteger is -255 in base 10, thenToHexString() returns ".....FFFFFFFFFF01", which is the 2's complement representation of -255.
  /// </example>
  /// <returns></returns>
  public string ToHexString()
  {
    string hexString = this.data[this.dataLength - 1].ToString("X");
    for (int index = this.dataLength - 2; index >= 0; --index)
      hexString += this.data[index].ToString("X8");
    return hexString;
  }

  /// <summary>Modulo Exponentiation</summary>
  /// <param name="exp">Exponential</param>
  /// <param name="n">Modulo</param>
  /// <returns>BigInteger result of raising this to the power of exp and then modulo n </returns>
  public BigInteger modPow(BigInteger exp, BigInteger n)
  {
    if (((int) exp.data[999] & int.MinValue) != 0)
      throw new ArithmeticException("Positive exponents only.");
    BigInteger bigInteger1 = (BigInteger) 1;
    bool flag = false;
    BigInteger bigInteger2;
    if (((int) this.data[999] & int.MinValue) != 0)
    {
      bigInteger2 = -this % n;
      flag = true;
    }
    else
      bigInteger2 = this % n;
    if (((int) n.data[999] & int.MinValue) != 0)
      n = -n;
    BigInteger bigInteger3 = new BigInteger();
    int index1 = n.dataLength << 1;
    bigInteger3.data[index1] = 1U;
    bigInteger3.dataLength = index1 + 1;
    BigInteger constant = bigInteger3 / n;
    int num1 = exp.bitCount();
    int num2 = 0;
    for (int index2 = 0; index2 < exp.dataLength; ++index2)
    {
      uint num3 = 1;
      for (int index3 = 0; index3 < 32; ++index3)
      {
        if (((int) exp.data[index2] & (int) num3) != 0)
          bigInteger1 = this.BarrettReduction(bigInteger1 * bigInteger2, n, constant);
        num3 <<= 1;
        bigInteger2 = this.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
        if (bigInteger2.dataLength == 1 && bigInteger2.data[0] == 1U)
          return flag && ((int) exp.data[0] & 1) != 0 ? -bigInteger1 : bigInteger1;
        ++num2;
        if (num2 == num1)
          break;
      }
    }
    return flag && ((int) exp.data[0] & 1) != 0 ? -bigInteger1 : bigInteger1;
  }

  /// <summary>
  /// Fast calculation of modular reduction using Barrett's reduction
  /// </summary>
  /// <remarks>
  /// Requires x &lt; b^(2k), where b is the base.  In this case, base is 2^32 (uint).
  /// 
  /// Reference [4]
  /// </remarks>
  /// <param name="x"></param>
  /// <param name="n"></param>
  /// <param name="constant"></param>
  /// <returns></returns>
  private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
  {
    int dataLength = n.dataLength;
    int index1 = dataLength + 1;
    int num1 = dataLength - 1;
    BigInteger bigInteger1 = new BigInteger();
    int index2 = num1;
    int index3 = 0;
    while (index2 < x.dataLength)
    {
      bigInteger1.data[index3] = x.data[index2];
      ++index2;
      ++index3;
    }
    bigInteger1.dataLength = x.dataLength - num1;
    if (bigInteger1.dataLength <= 0)
      bigInteger1.dataLength = 1;
    BigInteger bigInteger2 = bigInteger1 * constant;
    BigInteger bigInteger3 = new BigInteger();
    int index4 = index1;
    int index5 = 0;
    while (index4 < bigInteger2.dataLength)
    {
      bigInteger3.data[index5] = bigInteger2.data[index4];
      ++index4;
      ++index5;
    }
    bigInteger3.dataLength = bigInteger2.dataLength - index1;
    if (bigInteger3.dataLength <= 0)
      bigInteger3.dataLength = 1;
    BigInteger bigInteger4 = new BigInteger();
    int num2 = x.dataLength > index1 ? index1 : x.dataLength;
    for (int index6 = 0; index6 < num2; ++index6)
      bigInteger4.data[index6] = x.data[index6];
    bigInteger4.dataLength = num2;
    BigInteger bigInteger5 = new BigInteger();
    for (int index7 = 0; index7 < bigInteger3.dataLength; ++index7)
    {
      if (bigInteger3.data[index7] != 0U)
      {
        ulong num3 = 0;
        int index8 = index7;
        for (int index9 = 0; index9 < n.dataLength && index8 < index1; ++index8)
        {
          ulong num4 = (ulong) bigInteger3.data[index7] * (ulong) n.data[index9] + (ulong) bigInteger5.data[index8] + num3;
          bigInteger5.data[index8] = (uint) (num4 & (ulong) uint.MaxValue);
          num3 = num4 >> 32;
          ++index9;
        }
        if (index8 < index1)
          bigInteger5.data[index8] = (uint) num3;
      }
    }
    bigInteger5.dataLength = index1;
    while (bigInteger5.dataLength > 1 && bigInteger5.data[bigInteger5.dataLength - 1] == 0U)
      --bigInteger5.dataLength;
    BigInteger bigInteger6 = bigInteger4 - bigInteger5;
    if (((int) bigInteger6.data[999] & int.MinValue) != 0)
    {
      BigInteger bigInteger7 = new BigInteger();
      bigInteger7.data[index1] = 1U;
      bigInteger7.dataLength = index1 + 1;
      bigInteger6 += bigInteger7;
    }
    while (bigInteger6 >= n)
      bigInteger6 -= n;
    return bigInteger6;
  }

  /// <summary>Returns gcd(this, bi)</summary>
  /// <param name="bi"></param>
  /// <returns>Greatest Common Divisor of this and bi</returns>
  public BigInteger gcd(BigInteger bi)
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    BigInteger bigInteger2 = ((int) bi.data[999] & int.MinValue) == 0 ? bi : -bi;
    BigInteger bigInteger3 = bigInteger2;
    while (bigInteger1.dataLength > 1 || bigInteger1.dataLength == 1 && bigInteger1.data[0] != 0U)
    {
      bigInteger3 = bigInteger1;
      bigInteger1 = bigInteger2 % bigInteger1;
      bigInteger2 = bigInteger3;
    }
    return bigInteger3;
  }

  /// <summary>
  /// Populates "this" with the specified amount of random bits
  /// </summary>
  /// <param name="bits"></param>
  /// <param name="rand"></param>
  public void genRandomBits(int bits, Random rand)
  {
    int num1 = bits >> 5;
    int num2 = bits & 31;
    if (num2 != 0)
      ++num1;
    byte[] buffer = num1 <= 1000 && bits > 0 ? new byte[num1 * 4] : throw new ArithmeticException("Number of required bits is not valid.");
    rand.NextBytes(buffer);
    for (int index = 0; index < num1; ++index)
      this.data[index] = BitConverter.ToUInt32(buffer, index * 4);
    for (int index = num1; index < 1000; ++index)
      this.data[index] = 0U;
    if (num2 != 0)
    {
      if (bits != 1)
      {
        uint num3 = (uint) (1 << num2 - 1);
        this.data[num1 - 1] |= num3;
      }
      uint num4 = uint.MaxValue >> 32 - num2;
      this.data[num1 - 1] &= num4;
    }
    else
      this.data[num1 - 1] |= 2147483648U;
    this.dataLength = num1;
    if (this.dataLength != 0)
      return;
    this.dataLength = 1;
  }

  /// <summary>
  /// Returns the position of the most significant bit in the BigInteger
  /// </summary>
  /// <example>
  /// 1) The result is 1, if the value of BigInteger is 0...0000 0000
  /// 2) The result is 1, if the value of BigInteger is 0...0000 0001
  /// 3) The result is 2, if the value of BigInteger is 0...0000 0010
  /// 4) The result is 2, if the value of BigInteger is 0...0000 0011
  /// 5) The result is 5, if the value of BigInteger is 0...0001 0011
  /// </example>
  /// <returns></returns>
  public int bitCount()
  {
    while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
      --this.dataLength;
    uint num1 = this.data[this.dataLength - 1];
    uint num2 = 2147483648;
    int num3;
    for (num3 = 32; num3 > 0 && ((int) num1 & (int) num2) == 0; num2 >>= 1)
      --num3;
    int num4 = num3 + (this.dataLength - 1 << 5);
    return num4 != 0 ? num4 : 1;
  }

  /// <summary>
  /// Probabilistic prime test based on Fermat's little theorem
  /// </summary>
  /// <remarks>
  /// for any a &lt; p (p does not divide a) if
  ///      a^(p-1) mod p != 1 then p is not prime.
  /// 
  /// Otherwise, p is probably prime (pseudoprime to the chosen base).
  /// 
  /// This method is fast but fails for Carmichael numbers when the randomly chosen base is a factor of the number.
  /// </remarks>
  /// <param name="confidence">Number of chosen bases</param>
  /// <returns>True if this is a pseudoprime to randomly chosen bases</returns>
  public bool FermatLittleTest(int confidence)
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    if (bigInteger1.dataLength == 1)
    {
      if (bigInteger1.data[0] == 0U || bigInteger1.data[0] == 1U)
        return false;
      if (bigInteger1.data[0] == 2U || bigInteger1.data[0] == 3U)
        return true;
    }
    if (((int) bigInteger1.data[0] & 1) == 0)
      return false;
    int num = bigInteger1.bitCount();
    BigInteger bigInteger2 = new BigInteger();
    BigInteger exp = bigInteger1 - new BigInteger(1L);
    Random rand = new Random();
    for (int index = 0; index < confidence; ++index)
    {
      bool flag = false;
      while (!flag)
      {
        int bits = 0;
        while (bits < 2)
          bits = (int) (rand.NextDouble() * (double) num);
        bigInteger2.genRandomBits(bits, rand);
        int dataLength = bigInteger2.dataLength;
        if (dataLength > 1 || dataLength == 1 && bigInteger2.data[0] != 1U)
          flag = true;
      }
      BigInteger bigInteger3 = bigInteger2.gcd(bigInteger1);
      if (bigInteger3.dataLength == 1 && bigInteger3.data[0] != 1U)
        return false;
      BigInteger bigInteger4 = bigInteger2.modPow(exp, bigInteger1);
      int dataLength1 = bigInteger4.dataLength;
      if (dataLength1 > 1 || dataLength1 == 1 && bigInteger4.data[0] != 1U)
        return false;
    }
    return true;
  }

  /// <summary>Probabilistic prime test based on Rabin-Miller's</summary>
  /// <remarks>
  /// for any p &gt; 0 with p - 1 = 2^s * t
  /// 
  /// p is probably prime (strong pseudoprime) if for any a &lt; p,
  /// 1) a^t mod p = 1 or
  /// 2) a^((2^j)*t) mod p = p-1 for some 0 &lt;= j &lt;= s-1
  /// 
  /// Otherwise, p is composite.
  /// </remarks>
  /// <param name="confidence">Number of chosen bases</param>
  /// <returns>True if this is a strong pseudoprime to randomly chosen bases</returns>
  public bool RabinMillerTest(int confidence)
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    if (bigInteger1.dataLength == 1)
    {
      if (bigInteger1.data[0] == 0U || bigInteger1.data[0] == 1U)
        return false;
      if (bigInteger1.data[0] == 2U || bigInteger1.data[0] == 3U)
        return true;
    }
    if (((int) bigInteger1.data[0] & 1) == 0)
      return false;
    BigInteger bigInteger2 = bigInteger1 - new BigInteger(1L);
    int num1 = 0;
    for (int index1 = 0; index1 < bigInteger2.dataLength; ++index1)
    {
      uint num2 = 1;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        if (((int) bigInteger2.data[index1] & (int) num2) != 0)
        {
          index1 = bigInteger2.dataLength;
          break;
        }
        num2 <<= 1;
        ++num1;
      }
    }
    BigInteger exp = bigInteger2 >> num1;
    int num3 = bigInteger1.bitCount();
    BigInteger bigInteger3 = new BigInteger();
    Random rand = new Random();
    for (int index3 = 0; index3 < confidence; ++index3)
    {
      bool flag1 = false;
      while (!flag1)
      {
        int bits = 0;
        while (bits < 2)
          bits = (int) (rand.NextDouble() * (double) num3);
        bigInteger3.genRandomBits(bits, rand);
        int dataLength = bigInteger3.dataLength;
        if (dataLength > 1 || dataLength == 1 && bigInteger3.data[0] != 1U)
          flag1 = true;
      }
      BigInteger bigInteger4 = bigInteger3.gcd(bigInteger1);
      if (bigInteger4.dataLength == 1 && bigInteger4.data[0] != 1U)
        return false;
      BigInteger bigInteger5 = bigInteger3.modPow(exp, bigInteger1);
      bool flag2 = false;
      if (bigInteger5.dataLength == 1 && bigInteger5.data[0] == 1U)
        flag2 = true;
      for (int index4 = 0; !flag2 && index4 < num1; ++index4)
      {
        if (bigInteger5 == bigInteger2)
        {
          flag2 = true;
          break;
        }
        bigInteger5 = bigInteger5 * bigInteger5 % bigInteger1;
      }
      if (!flag2)
        return false;
    }
    return true;
  }

  /// <summary>
  /// Probabilistic prime test based on Solovay-Strassen (Euler Criterion)
  /// </summary>
  /// <remarks>
  ///  p is probably prime if for any a &lt; p (a is not multiple of p),
  /// a^((p-1)/2) mod p = J(a, p)
  /// 
  /// where J is the Jacobi symbol.
  /// 
  /// Otherwise, p is composite.
  /// </remarks>
  /// <param name="confidence">Number of chosen bases</param>
  /// <returns>True if this is a Euler pseudoprime to randomly chosen bases</returns>
  public bool SolovayStrassenTest(int confidence)
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    if (bigInteger1.dataLength == 1)
    {
      if (bigInteger1.data[0] == 0U || bigInteger1.data[0] == 1U)
        return false;
      if (bigInteger1.data[0] == 2U || bigInteger1.data[0] == 3U)
        return true;
    }
    if (((int) bigInteger1.data[0] & 1) == 0)
      return false;
    int num = bigInteger1.bitCount();
    BigInteger a = new BigInteger();
    BigInteger bigInteger2 = bigInteger1 - (BigInteger) 1;
    BigInteger exp = bigInteger2 >> 1;
    Random rand = new Random();
    for (int index = 0; index < confidence; ++index)
    {
      bool flag = false;
      while (!flag)
      {
        int bits = 0;
        while (bits < 2)
          bits = (int) (rand.NextDouble() * (double) num);
        a.genRandomBits(bits, rand);
        int dataLength = a.dataLength;
        if (dataLength > 1 || dataLength == 1 && a.data[0] != 1U)
          flag = true;
      }
      BigInteger bigInteger3 = a.gcd(bigInteger1);
      if (bigInteger3.dataLength == 1 && bigInteger3.data[0] != 1U)
        return false;
      BigInteger bigInteger4 = a.modPow(exp, bigInteger1);
      if (bigInteger4 == bigInteger2)
        bigInteger4 = (BigInteger) -1;
      BigInteger bigInteger5 = (BigInteger) BigInteger.Jacobi(a, bigInteger1);
      if (bigInteger4 != bigInteger5)
        return false;
    }
    return true;
  }

  /// <summary>
  /// Implementation of the Lucas Strong Pseudo Prime test
  /// </summary>
  /// <remarks>
  /// Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
  /// with d odd and s &gt;= 0.
  /// 
  /// If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 &lt;= r &lt; s, then n
  /// is a strong Lucas pseudoprime with parameters (P, Q).  We select
  /// P and Q based on Selfridge.
  /// </remarks>
  /// <returns>True if number is a strong Lucus pseudo prime</returns>
  public bool LucasStrongTest()
  {
    BigInteger thisVal = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    if (thisVal.dataLength == 1)
    {
      if (thisVal.data[0] == 0U || thisVal.data[0] == 1U)
        return false;
      if (thisVal.data[0] == 2U || thisVal.data[0] == 3U)
        return true;
    }
    return ((int) thisVal.data[0] & 1) != 0 && this.LucasStrongTestHelper(thisVal);
  }

  private bool LucasStrongTestHelper(BigInteger thisVal)
  {
    long a = 5;
    long num1 = -1;
    long num2 = 0;
    bool flag1 = false;
    while (!flag1)
    {
      switch (BigInteger.Jacobi((BigInteger) a, thisVal))
      {
        case -1:
          flag1 = true;
          break;
        case 0:
          if ((BigInteger) Math.Abs(a) < thisVal)
            return false;
          goto default;
        default:
          if (num2 == 20L)
          {
            BigInteger bigInteger = thisVal.sqrt();
            if (bigInteger * bigInteger == thisVal)
              return false;
          }
          a = (Math.Abs(a) + 2L) * num1;
          num1 = -num1;
          break;
      }
      ++num2;
    }
    long num3 = 1L - a >> 2;
    BigInteger bigInteger1 = thisVal + (BigInteger) 1;
    int num4 = 0;
    for (int index1 = 0; index1 < bigInteger1.dataLength; ++index1)
    {
      uint num5 = 1;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        if (((int) bigInteger1.data[index1] & (int) num5) != 0)
        {
          index1 = bigInteger1.dataLength;
          break;
        }
        num5 <<= 1;
        ++num4;
      }
    }
    BigInteger k = bigInteger1 >> num4;
    BigInteger bigInteger2 = new BigInteger();
    int index3 = thisVal.dataLength << 1;
    bigInteger2.data[index3] = 1U;
    bigInteger2.dataLength = index3 + 1;
    BigInteger constant = bigInteger2 / thisVal;
    BigInteger[] bigIntegerArray1 = BigInteger.LucasSequenceHelper((BigInteger) 1, (BigInteger) num3, k, thisVal, constant, 0);
    bool flag2 = false;
    if (bigIntegerArray1[0].dataLength == 1 && bigIntegerArray1[0].data[0] == 0U || bigIntegerArray1[1].dataLength == 1 && bigIntegerArray1[1].data[0] == 0U)
      flag2 = true;
    for (int index4 = 1; index4 < num4; ++index4)
    {
      if (!flag2)
      {
        bigIntegerArray1[1] = thisVal.BarrettReduction(bigIntegerArray1[1] * bigIntegerArray1[1], thisVal, constant);
        bigIntegerArray1[1] = (bigIntegerArray1[1] - (bigIntegerArray1[2] << 1)) % thisVal;
        if (bigIntegerArray1[1].dataLength == 1 && bigIntegerArray1[1].data[0] == 0U)
          flag2 = true;
      }
      bigIntegerArray1[2] = thisVal.BarrettReduction(bigIntegerArray1[2] * bigIntegerArray1[2], thisVal, constant);
    }
    if (flag2)
    {
      BigInteger bigInteger3 = thisVal.gcd((BigInteger) num3);
      if (bigInteger3.dataLength == 1 && bigInteger3.data[0] == 1U)
      {
        if (((int) bigIntegerArray1[2].data[999] & int.MinValue) != 0)
        {
          BigInteger[] bigIntegerArray2 = bigIntegerArray1;
          bigIntegerArray2[2] = bigIntegerArray2[2] + thisVal;
        }
        BigInteger bigInteger4 = (BigInteger) (num3 * (long) BigInteger.Jacobi((BigInteger) num3, thisVal)) % thisVal;
        if (((int) bigInteger4.data[999] & int.MinValue) != 0)
          bigInteger4 += thisVal;
        if (bigIntegerArray1[2] != bigInteger4)
          flag2 = false;
      }
    }
    return flag2;
  }

  /// <summary>
  /// Determines whether a number is probably prime using the Rabin-Miller's test
  /// </summary>
  /// <remarks>
  /// Before applying the test, the number is tested for divisibility by primes &lt; 2000
  /// </remarks>
  /// <param name="confidence">Number of chosen bases</param>
  /// <returns>True if this is probably prime</returns>
  public bool isProbablePrime(int confidence)
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    for (int index = 0; index < BigInteger.primesBelow2000.Length; ++index)
    {
      BigInteger bigInteger2 = (BigInteger) BigInteger.primesBelow2000[index];
      if (!(bigInteger2 >= bigInteger1))
      {
        if ((bigInteger1 % bigInteger2).IntValue() == 0)
          return false;
      }
      else
        break;
    }
    return bigInteger1.RabinMillerTest(confidence);
  }

  /// <summary>
  /// Determines whether this BigInteger is probably prime using a combination of base 2 strong pseudoprime test and Lucas strong pseudoprime test
  /// </summary>
  /// <remarks>
  /// The sequence of the primality test is as follows,
  /// 
  /// 1) Trial divisions are carried out using prime numbers below 2000.
  ///    if any of the primes divides this BigInteger, then it is not prime.
  /// 
  /// 2) Perform base 2 strong pseudoprime test.  If this BigInteger is a
  ///    base 2 strong pseudoprime, proceed on to the next step.
  /// 
  /// 3) Perform strong Lucas pseudoprime test.
  /// 
  /// For a detailed discussion of this primality test, see [6].
  /// </remarks>
  /// <returns>True if this is probably prime</returns>
  public bool isProbablePrime()
  {
    BigInteger bigInteger1 = ((int) this.data[999] & int.MinValue) == 0 ? this : -this;
    if (bigInteger1.dataLength == 1)
    {
      if (bigInteger1.data[0] == 0U || bigInteger1.data[0] == 1U)
        return false;
      if (bigInteger1.data[0] == 2U || bigInteger1.data[0] == 3U)
        return true;
    }
    if (((int) bigInteger1.data[0] & 1) == 0)
      return false;
    for (int index = 0; index < BigInteger.primesBelow2000.Length; ++index)
    {
      BigInteger bigInteger2 = (BigInteger) BigInteger.primesBelow2000[index];
      if (!(bigInteger2 >= bigInteger1))
      {
        if ((bigInteger1 % bigInteger2).IntValue() == 0)
          return false;
      }
      else
        break;
    }
    BigInteger bigInteger3 = bigInteger1 - new BigInteger(1L);
    int num1 = 0;
    for (int index1 = 0; index1 < bigInteger3.dataLength; ++index1)
    {
      uint num2 = 1;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        if (((int) bigInteger3.data[index1] & (int) num2) != 0)
        {
          index1 = bigInteger3.dataLength;
          break;
        }
        num2 <<= 1;
        ++num1;
      }
    }
    BigInteger exp = bigInteger3 >> num1;
    bigInteger1.bitCount();
    BigInteger bigInteger4 = ((BigInteger) 2).modPow(exp, bigInteger1);
    bool flag = false;
    if (bigInteger4.dataLength == 1 && bigInteger4.data[0] == 1U)
      flag = true;
    for (int index = 0; !flag && index < num1; ++index)
    {
      if (bigInteger4 == bigInteger3)
      {
        flag = true;
        break;
      }
      bigInteger4 = bigInteger4 * bigInteger4 % bigInteger1;
    }
    if (flag)
      flag = this.LucasStrongTestHelper(bigInteger1);
    return flag;
  }

  /// <summary>
  /// Returns the lowest 4 bytes of the BigInteger as an int
  /// </summary>
  /// <returns>Lowest 4 bytes as integer</returns>
  public int IntValue() => (int) this.data[0];

  /// <summary>
  /// Returns the lowest 8 bytes of the BigInteger as a long
  /// </summary>
  /// <returns>Lowest 8 bytes as long</returns>
  public long LongValue()
  {
    long num = (long) this.data[0];
    try
    {
      num |= (long) this.data[1] << 32;
    }
    catch (Exception ex)
    {
      if (((int) this.data[0] & int.MinValue) != 0)
        num = (long) (int) this.data[0];
    }
    return num;
  }

  /// <summary>Computes the Jacobi Symbol for 2 BigInteger a and b</summary>
  /// <remarks>
  /// Algorithm adapted from [3] and [4] with some optimizations
  /// </remarks>
  /// <param name="a">Any BigInteger</param>
  /// <param name="b">Odd BigInteger</param>
  /// <returns>Jacobi Symbol</returns>
  public static int Jacobi(BigInteger a, BigInteger b)
  {
    if (((int) b.data[0] & 1) == 0)
      throw new ArgumentException("Jacobi defined only for odd integers.");
    if (a >= b)
      a %= b;
    if (a.dataLength == 1 && a.data[0] == 0U)
      return 0;
    if (a.dataLength == 1 && a.data[0] == 1U)
      return 1;
    if (a < (BigInteger) 0)
      return ((int) (b - (BigInteger) 1).data[0] & 2) == 0 ? BigInteger.Jacobi(-a, b) : -BigInteger.Jacobi(-a, b);
    int num1 = 0;
    for (int index1 = 0; index1 < a.dataLength; ++index1)
    {
      uint num2 = 1;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        if (((int) a.data[index1] & (int) num2) != 0)
        {
          index1 = a.dataLength;
          break;
        }
        num2 <<= 1;
        ++num1;
      }
    }
    BigInteger b1 = a >> num1;
    int num3 = 1;
    if ((num1 & 1) != 0 && (((int) b.data[0] & 7) == 3 || ((int) b.data[0] & 7) == 5))
      num3 = -1;
    if (((int) b.data[0] & 3) == 3 && ((int) b1.data[0] & 3) == 3)
      num3 = -num3;
    return b1.dataLength == 1 && b1.data[0] == 1U ? num3 : num3 * BigInteger.Jacobi(b % b1, b1);
  }

  /// <summary>
  /// Generates a positive BigInteger that is probably prime
  /// </summary>
  /// <param name="bits">Number of bit</param>
  /// <param name="confidence">Number of chosen bases</param>
  /// <param name="rand">Random object</param>
  /// <returns>A probably prime number</returns>
  public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
  {
    BigInteger bigInteger = new BigInteger();
    for (bool flag = false; !flag; flag = bigInteger.isProbablePrime(confidence))
    {
      bigInteger.genRandomBits(bits, rand);
      bigInteger.data[0] |= 1U;
    }
    return bigInteger;
  }

  /// <summary>
  /// Generates a random number with the specified number of bits such that gcd(number, this) = 1
  /// </summary>
  /// <remarks>
  /// The number of bits must be greater than 0 and less than 2209
  /// </remarks>
  /// <param name="bits">Number of bit</param>
  /// <param name="rand">Random object</param>
  /// <returns>Relatively prime number of this</returns>
  public BigInteger genCoPrime(int bits, Random rand)
  {
    bool flag = false;
    BigInteger bigInteger1 = new BigInteger();
    while (!flag)
    {
      bigInteger1.genRandomBits(bits, rand);
      BigInteger bigInteger2 = bigInteger1.gcd(this);
      if (bigInteger2.dataLength == 1 && bigInteger2.data[0] == 1U)
        flag = true;
    }
    return bigInteger1;
  }

  /// <summary>Returns the modulo inverse of this</summary>
  /// <remarks>
  /// Throws ArithmeticException if the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
  /// </remarks>
  /// <param name="modulus"></param>
  /// <returns>Modulo inverse of this</returns>
  public BigInteger modInverse(BigInteger modulus)
  {
    BigInteger[] bigIntegerArray1 = new BigInteger[2]
    {
      (BigInteger) 0,
      (BigInteger) 1
    };
    BigInteger[] bigIntegerArray2 = new BigInteger[2];
    BigInteger[] bigIntegerArray3 = new BigInteger[2]
    {
      (BigInteger) 0,
      (BigInteger) 0
    };
    int num = 0;
    BigInteger bi1 = modulus;
    BigInteger bi2 = this;
    while (bi2.dataLength > 1 || bi2.dataLength == 1 && bi2.data[0] != 0U)
    {
      BigInteger outQuotient = new BigInteger();
      BigInteger outRemainder = new BigInteger();
      if (num > 1)
      {
        BigInteger bigInteger = (bigIntegerArray1[0] - bigIntegerArray1[1] * bigIntegerArray2[0]) % modulus;
        bigIntegerArray1[0] = bigIntegerArray1[1];
        bigIntegerArray1[1] = bigInteger;
      }
      if (bi2.dataLength == 1)
        BigInteger.singleByteDivide(bi1, bi2, outQuotient, outRemainder);
      else
        BigInteger.multiByteDivide(bi1, bi2, outQuotient, outRemainder);
      bigIntegerArray2[0] = bigIntegerArray2[1];
      bigIntegerArray3[0] = bigIntegerArray3[1];
      bigIntegerArray2[1] = outQuotient;
      bigIntegerArray3[1] = outRemainder;
      bi1 = bi2;
      bi2 = outRemainder;
      ++num;
    }
    if (bigIntegerArray3[0].dataLength > 1 || bigIntegerArray3[0].dataLength == 1 && bigIntegerArray3[0].data[0] != 1U)
      throw new ArithmeticException("No inverse!");
    BigInteger bigInteger1 = (bigIntegerArray1[0] - bigIntegerArray1[1] * bigIntegerArray2[0]) % modulus;
    if (((int) bigInteger1.data[999] & int.MinValue) != 0)
      bigInteger1 += modulus;
    return bigInteger1;
  }

  /// <summary>Returns the value of the BigInteger as a byte array</summary>
  /// <remarks>The lowest index contains the MSB</remarks>
  /// <returns>Byte array containing value of the BigInteger</returns>
  public byte[] getBytes()
  {
    int num1 = this.bitCount();
    int length = num1 >> 3;
    if ((num1 & 7) != 0)
      ++length;
    byte[] bytes = new byte[length];
    int index1 = 0;
    uint num2 = this.data[this.dataLength - 1];
    uint num3;
    if ((num3 = num2 >> 24 & (uint) byte.MaxValue) != 0U)
      bytes[index1++] = (byte) num3;
    uint num4;
    if ((num4 = num2 >> 16 & (uint) byte.MaxValue) != 0U)
      bytes[index1++] = (byte) num4;
    else if (index1 > 0)
      ++index1;
    uint num5;
    if ((num5 = num2 >> 8 & (uint) byte.MaxValue) != 0U)
      bytes[index1++] = (byte) num5;
    else if (index1 > 0)
      ++index1;
    uint num6;
    if ((num6 = num2 & (uint) byte.MaxValue) != 0U)
      bytes[index1++] = (byte) num6;
    else if (index1 > 0)
      ++index1;
    int index2 = this.dataLength - 2;
    while (index2 >= 0)
    {
      uint num7 = this.data[index2];
      bytes[index1 + 3] = (byte) (num7 & (uint) byte.MaxValue);
      uint num8 = num7 >> 8;
      bytes[index1 + 2] = (byte) (num8 & (uint) byte.MaxValue);
      uint num9 = num8 >> 8;
      bytes[index1 + 1] = (byte) (num9 & (uint) byte.MaxValue);
      uint num10 = num9 >> 8;
      bytes[index1] = (byte) (num10 & (uint) byte.MaxValue);
      --index2;
      index1 += 4;
    }
    return bytes;
  }

  /// <summary>Sets the value of the specified bit to 1</summary>
  /// <remarks>The Least Significant Bit position is 0</remarks>
  /// <param name="bitNum">The position of bit to be changed</param>
  public void setBit(uint bitNum)
  {
    uint index = bitNum >> 5;
    uint num = 1U << (int) (byte) (bitNum & 31U);
    this.data[(int) index] |= num;
    if ((long) index < (long) this.dataLength)
      return;
    this.dataLength = (int) index + 1;
  }

  /// <summary>Sets the value of the specified bit to 0</summary>
  /// <remarks>The Least Significant Bit position is 0</remarks>
  /// <param name="bitNum">The position of bit to be changed</param>
  public void unsetBit(uint bitNum)
  {
    uint index = bitNum >> 5;
    if ((long) index >= (long) this.dataLength)
      return;
    uint num = uint.MaxValue ^ 1U << (int) (byte) (bitNum & 31U);
    this.data[(int) index] &= num;
    if (this.dataLength <= 1 || this.data[this.dataLength - 1] != 0U)
      return;
    --this.dataLength;
  }

  /// <summary>
  /// Returns a value that is equivalent to the integer square root of this
  /// </summary>
  /// <remarks>
  /// The integer square root of "this" is defined as the largest integer n, such that (n * n) &lt;= this.
  /// Square root of negative integer is an undefined behaviour (UB).
  /// </remarks>
  /// <returns>Integer square root of this</returns>
  public BigInteger sqrt()
  {
    uint num1 = (uint) this.bitCount();
    uint num2 = ((int) num1 & 1) == 0 ? num1 >> 1 : (num1 >> 1) + 1U;
    uint num3 = num2 >> 5;
    byte num4 = (byte) (num2 & 31U);
    BigInteger bigInteger = new BigInteger();
    uint num5;
    if (num4 == (byte) 0)
    {
      num5 = 2147483648U;
    }
    else
    {
      num5 = 1U << (int) num4;
      ++num3;
    }
    bigInteger.dataLength = (int) num3;
    for (int index = (int) num3 - 1; index >= 0; --index)
    {
      for (; num5 != 0U; num5 >>= 1)
      {
        bigInteger.data[index] ^= num5;
        if (bigInteger * bigInteger > this)
          bigInteger.data[index] ^= num5;
      }
      num5 = 2147483648U;
    }
    return bigInteger;
  }

  /// <summary>
  /// Returns the k_th number in the Lucas Sequence reduced modulo n
  /// </summary>
  /// <remarks>
  /// Uses index doubling to speed up the process.  For example, to calculate V(k),
  /// we maintain two numbers in the sequence V(n) and V(n+1).
  /// 
  /// To obtain V(2n), we use the identity
  ///      V(2n) = (V(n) * V(n)) - (2 * Q^n)
  /// To obtain V(2n+1), we first write it as
  ///      V(2n+1) = V((n+1) + n)
  /// and use the identity
  ///      V(m+n) = V(m) * V(n) - Q * V(m-n)
  /// Hence,
  ///      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
  ///                   = V(n+1) * V(n) - Q^n * V(1)
  ///                   = V(n+1) * V(n) - Q^n * P
  /// 
  /// We use k in its binary expansion and perform index doubling for each
  /// bit position.  For each bit position that is set, we perform an
  /// index doubling followed by an index addition.  This means that for V(n),
  /// we need to update it to V(2n+1).  For V(n+1), we need to update it to
  /// V((2n+1)+1) = V(2*(n+1))
  /// 
  /// This function returns
  /// [0] = U(k)
  /// [1] = V(k)
  /// [2] = Q^n
  /// 
  /// Where U(0) = 0 % n, U(1) = 1 % n
  ///       V(0) = 2 % n, V(1) = P % n
  /// </remarks>
  /// <param name="P"></param>
  /// <param name="Q"></param>
  /// <param name="k"></param>
  /// <param name="n"></param>
  /// <returns></returns>
  public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q, BigInteger k, BigInteger n)
  {
    if (k.dataLength == 1 && k.data[0] == 0U)
      return new BigInteger[3]
      {
        (BigInteger) 0,
        (BigInteger) 2 % n,
        (BigInteger) 1 % n
      };
    BigInteger bigInteger = new BigInteger();
    int index1 = n.dataLength << 1;
    bigInteger.data[index1] = 1U;
    bigInteger.dataLength = index1 + 1;
    BigInteger constant = bigInteger / n;
    int s = 0;
    for (int index2 = 0; index2 < k.dataLength; ++index2)
    {
      uint num = 1;
      for (int index3 = 0; index3 < 32; ++index3)
      {
        if (((int) k.data[index2] & (int) num) != 0)
        {
          index2 = k.dataLength;
          break;
        }
        num <<= 1;
        ++s;
      }
    }
    BigInteger k1 = k >> s;
    return BigInteger.LucasSequenceHelper(P, Q, k1, n, constant, s);
  }

  private static BigInteger[] LucasSequenceHelper(
    BigInteger P,
    BigInteger Q,
    BigInteger k,
    BigInteger n,
    BigInteger constant,
    int s)
  {
    BigInteger[] bigIntegerArray = new BigInteger[3];
    if (((int) k.data[0] & 1) == 0)
      throw new ArgumentException("Argument k must be odd.");
    uint num = (uint) (1 << (k.bitCount() & 31) - 1);
    BigInteger bigInteger1 = (BigInteger) 2 % n;
    BigInteger bigInteger2 = (BigInteger) 1 % n;
    BigInteger bigInteger3 = P % n;
    BigInteger bigInteger4 = bigInteger2;
    bool flag = true;
    for (int index = k.dataLength - 1; index >= 0; --index)
    {
      for (; num != 0U && (index != 0 || num != 1U); num >>= 1)
      {
        if (((int) k.data[index] & (int) num) != 0)
        {
          bigInteger4 = bigInteger4 * bigInteger3 % n;
          bigInteger1 = (bigInteger1 * bigInteger3 - P * bigInteger2) % n;
          bigInteger3 = (n.BarrettReduction(bigInteger3 * bigInteger3, n, constant) - (bigInteger2 * Q << 1)) % n;
          if (flag)
            flag = false;
          else
            bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
          bigInteger2 = bigInteger2 * Q % n;
        }
        else
        {
          bigInteger4 = (bigInteger4 * bigInteger1 - bigInteger2) % n;
          bigInteger3 = (bigInteger1 * bigInteger3 - P * bigInteger2) % n;
          bigInteger1 = (n.BarrettReduction(bigInteger1 * bigInteger1, n, constant) - (bigInteger2 << 1)) % n;
          if (flag)
          {
            bigInteger2 = Q % n;
            flag = false;
          }
          else
            bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
        }
      }
      num = 2147483648U;
    }
    BigInteger bigInteger5 = (bigInteger4 * bigInteger1 - bigInteger2) % n;
    BigInteger bigInteger6 = (bigInteger1 * bigInteger3 - P * bigInteger2) % n;
    if (flag)
      flag = false;
    else
      bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
    BigInteger bigInteger7 = bigInteger2 * Q % n;
    for (int index = 0; index < s; ++index)
    {
      bigInteger5 = bigInteger5 * bigInteger6 % n;
      bigInteger6 = (bigInteger6 * bigInteger6 - (bigInteger7 << 1)) % n;
      if (flag)
      {
        bigInteger7 = Q % n;
        flag = false;
      }
      else
        bigInteger7 = n.BarrettReduction(bigInteger7 * bigInteger7, n, constant);
    }
    bigIntegerArray[0] = bigInteger5;
    bigIntegerArray[1] = bigInteger6;
    bigIntegerArray[2] = bigInteger7;
    return bigIntegerArray;
  }
}
