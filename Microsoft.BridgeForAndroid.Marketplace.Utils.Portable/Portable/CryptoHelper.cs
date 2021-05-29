// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.CryptoHelper
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;
using System.Globalization;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public static class CryptoHelper
  {
    public static byte[] ComputeMD5Hash(byte[] input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      byte[] numArray;
      CryptographicBuffer.CopyToByteArray(HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5).HashData(CryptographicBuffer.CreateFromByteArray(input)), ref numArray);
      return numArray;
    }

    public static string ComputeMD5HashAsHexadecimal(byte[] input)
    {
      byte[] numArray = input != null ? CryptoHelper.ComputeMD5Hash(input) : throw new ArgumentNullException(nameof (input));
      StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
      for (int index = 0; index < numArray.Length; ++index)
        stringBuilder.Append(numArray[index].ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }

    public static byte[] ComputeSha1Hash(byte[] input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      byte[] numArray;
      CryptographicBuffer.CopyToByteArray(HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1).HashData(CryptographicBuffer.CreateFromByteArray(input)), ref numArray);
      return numArray;
    }
  }
}
