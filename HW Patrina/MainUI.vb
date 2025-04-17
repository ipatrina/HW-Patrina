Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading

Public Class MainUI

    <Flags()>
    Public Enum MenuFlags As Integer
        MF_BYPOSITION = 1024
        MF_REMOVE = 4096
        MF_SEPARATOR = 2048
        MF_STRING = 0
    End Enum

    <DllImport("user32.dll", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function GetSystemMenu(hWnd As IntPtr, Optional bRevert As Boolean = False) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function AppendMenu(hMenu As IntPtr, uFlags As MenuFlags, uIDNewItem As Int32, lpNewItem As String) As Boolean
    End Function

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = 274 Then
            If m.WParam.ToInt32 = &H1FFE Then
                MsgBox("此软件包含的开源软件由权利持有人发放许可证。" & vbCrLf & vbCrLf & "软件： aescrypt2 1.0" & vbCrLf & "版权声明： Copyright (C) 2004,2005  Christophe Devine" & vbCrLf & "许可证： GPL v2 License" & vbCrLf & vbCrLf & "许可证文本： https://www.gnu.org/licenses/gpl-2.0.txt", vbInformation, "开源软件使用声明")
            End If

            If m.WParam.ToInt32 = &H1FFF Then
                Dim VersionStrings As String() = Application.ProductVersion.ToString.Split(".")
                MsgBox("HW Patrina" & vbCrLf & vbCrLf & "华为ONT配置文件实用工具" & vbCrLf & vbCrLf & "软件版本：" & VersionStrings(0) & "." & VersionStrings(1) & "." & VersionStrings(2) & vbCrLf & "更新时间：20" & VersionStrings(3).Substring(0, 2) & "年" & Int(VersionStrings(3).Substring(2, 2)) & "月" & vbCrLf & vbCrLf & "Copyright © 2022-2025 版权所有", vbInformation, "关于")
            End If
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub BTN_BROWSE_Click(sender As Object, e As EventArgs) Handles BTN_BROWSE.Click
        Try
            If OFD_CONFIG.ShowDialog = DialogResult.OK Then
                LoadConfig(OFD_CONFIG.FileName)
            Else
                ResetBanner()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Try
            LoadConfig(e.Data.GetData(DataFormats.FileDrop)(0))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            Dispose()
            End
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            AppendMenu(GetSystemMenu(Handle), MenuFlags.MF_SEPARATOR, &H1FFD, "SEPARATOR")
            AppendMenu(GetSystemMenu(Handle), MenuFlags.MF_STRING, &H1FFE, "开源软件使用声明(&O)")
            AppendMenu(GetSystemMenu(Handle), MenuFlags.MF_STRING, &H1FFF, "关于(&A)")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TMR_BANNER_Tick(sender As Object, e As EventArgs) Handles TMR_BANNER.Tick
        If BannerTimer = 0 Or BannerTimer = 2 Or BannerTimer = 4 Then
            If IsResultOK = True Then
                LBL_BANNER.BackColor = Color.Green
            Else
                LBL_BANNER.BackColor = Color.Red
            End If
            If BannerTimer = 4 Then
                TMR_BANNER.Enabled = False
            End If
        Else
            LBL_BANNER.BackColor = Color.White
        End If
        BannerTimer += 1
    End Sub

    Private Function AesCrypt2(param1 As Byte(), param2 As Integer, param3 As Byte()) As Byte()
        Dim TempPath As String = Path.GetTempPath()
        Dim Aescrypt2_EXE As String = TempPath & "aescrypt2.exe"
        If Not My.Computer.FileSystem.FileExists(Aescrypt2_EXE) Then ReleaseResource("HW_Patrina.aescrypt2.exe", Aescrypt2_EXE)
        Dim InputFile As String = Path.GetTempFileName()
        Dim OutputFile As String = Path.GetTempFileName()
        Dim KeyFile As String = Path.GetTempFileName()
        My.Computer.FileSystem.WriteAllBytes(InputFile, param1, False)
        My.Computer.FileSystem.WriteAllBytes(KeyFile, param3, False)
        Dim _loc_1 As New Process()
        _loc_1.StartInfo.FileName = Aescrypt2_EXE
        _loc_1.StartInfo.Arguments = Chr(34) & param2.ToString() & Chr(34) & " " & Chr(34) & InputFile.Replace("\", "/") & Chr(34) & " " & Chr(34) & OutputFile.Replace("\", "/") & Chr(34) & " " & Chr(34) & KeyFile & Chr(34)
        _loc_1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        _loc_1.Start()
        _loc_1.WaitForExit(10000)
        Try
            _loc_1.Kill()
        Catch ex As Exception

        End Try
        _loc_1.Close()
        Dim _loc_2 As Byte() = My.Computer.FileSystem.ReadAllBytes(OutputFile)
        My.Computer.FileSystem.DeleteFile(InputFile)
        My.Computer.FileSystem.DeleteFile(KeyFile)
        My.Computer.FileSystem.DeleteFile(OutputFile)
        Return _loc_2
    End Function

    Public Function BinToBytes(param1 As String) As Byte()
        Dim _loc_1 As Integer = param1.Length / 8
        Dim _loc_2 As Byte() = New Byte(_loc_1 - 1) {}
        For _loc_3 As Integer = 0 To _loc_1 - 1
            _loc_2(_loc_3) = Convert.ToByte(param1.Substring(8 * _loc_3, 8), 2)
        Next
        Return _loc_2
    End Function

    Public Function BytesToBin(param1() As Byte) As String
        Dim _loc_1 As New StringBuilder
        For Each _loc_2 In param1
            _loc_1.Append(Convert.ToString(_loc_2, 2).PadLeft(8, "0"))
        Next
        Return _loc_1.ToString
    End Function

    Public Function BytesToHex(param1 As Byte()) As String
        Return BitConverter.ToString(param1).Replace("-", "").ToUpper
    End Function

    Public Function BytesToInt32(param1 As Byte()) As Integer
        Return Int(param1(0)) + Int(param1(1)) * 256 + Int(param1(2)) * 65536 + Int(param1(3)) * 16777216
    End Function

    Public Function CRC32(param1 As Byte()) As Byte()
        Dim _loc_1 As UInteger() = New UInteger(255) {}
        For _loc_2 As Integer = 0 To 255
            Dim _loc_3 As UInteger = _loc_2
            For _loc_4 As Integer = 8 To 1 Step -1
                If (_loc_3 And &H1) = 1 Then
                    _loc_3 = (_loc_3 >> 1) And &H7FFFFFFFUI
                    _loc_3 = _loc_3 Xor &HEDB88320UI
                Else
                    _loc_3 >>= 1
                End If
            Next
            _loc_1(_loc_2) = _loc_3
        Next
        Dim _loc_5 As UInteger = &HFFFFFFFFUI
        For Each _loc_6 As Byte In param1
            Dim _loc_7 As Integer = (_loc_5 And &HFF) Xor _loc_6
            _loc_5 = (_loc_5 >> 8) Xor _loc_1(_loc_7)
        Next
        _loc_5 = Not _loc_5
        Return BitConverter.GetBytes(_loc_5)
    End Function

    Private Function DecryptAES(Input As Byte(), Key As Byte(), IV As Byte()) As Byte()
        Dim Decryptor As System.Security.Cryptography.Aes = System.Security.Cryptography.Aes.Create("AES")
        Decryptor.BlockSize = 128
        Decryptor.KeySize = 256
        Decryptor.Key = Key
        Decryptor.IV = IV
        Decryptor.Mode = CipherMode.CBC
        Decryptor.Padding = PaddingMode.Zeros
        Return Decryptor.CreateDecryptor().TransformFinalBlock(Input, 0, Input.Length)
    End Function

    Private Function EncryptAES(Input As Byte(), Key As Byte(), IV As Byte()) As Byte()
        Dim Encryptor As System.Security.Cryptography.Aes = System.Security.Cryptography.Aes.Create("AES")
        Encryptor.BlockSize = 128
        Encryptor.KeySize = 256
        Encryptor.Key = Key
        Encryptor.IV = IV
        Encryptor.Mode = CipherMode.CBC
        Encryptor.Padding = PaddingMode.Zeros
        Return Encryptor.CreateEncryptor().TransformFinalBlock(Input, 0, Input.Length)
    End Function

    Private Function GetCtce8File(param1 As Byte()) As Byte()
        Dim Ctce8Magic As Byte() = New Byte() {&H67, &H66, &H63, &H71}
        Dim Ctce8Payload As Byte() = GZip(param1)
        Dim Ctce8CRC As Byte() = CRC32(Ctce8Payload)
        Dim Ctce8OutputBuffer As Byte() = New Byte(Ctce8Payload.Length + 32 - 1) {}
        Array.Copy(Ctce8Magic, 0, Ctce8OutputBuffer, 0, Ctce8Magic.Length)
        Array.Copy(Ctce8CRC, 0, Ctce8OutputBuffer, 4, Ctce8CRC.Length)
        Array.Copy(BitConverter.GetBytes(Convert.ToInt32(Ctce8Payload.Length)), 0, Ctce8OutputBuffer, 8, 4)
        Array.Copy(BitConverter.GetBytes(Convert.ToInt32(Time())), 0, Ctce8OutputBuffer, 12, 4)
        Array.Copy(Ctce8Payload, 0, Ctce8OutputBuffer, 32, Ctce8Payload.Length)
        Return Ctce8OutputBuffer
    End Function

    Public Function GetRndString(StringLength As Long) As String
        Try
            Randomize()
            Dim _loc_1 As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            Dim _loc_2 As New Random(GetSeed())
            Dim _loc_3 As New StringBuilder
            For _loc_4 As Integer = 1 To StringLength
                _loc_3.Append(_loc_1.Substring(_loc_2.Next(0, _loc_1.Length - 1), 1))
            Next
            Return _loc_3.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function GetSeed() As Integer
        Dim _loc_1 As Byte() = New Byte(3) {}
        Dim _loc_2 As New RNGCryptoServiceProvider()
        _loc_2.GetBytes(_loc_1)
        Return System.Math.Abs(BitConverter.ToInt32(_loc_1, 0))
    End Function

    Public Function GZip(param1 As Byte()) As Byte()
        Dim _loc_1 As Byte() = param1
        Dim _loc_2 As New MemoryStream()
        Using _loc_3 As New Compression.GZipStream(_loc_2, CompressionMode.Compress, True)
            _loc_3.Write(_loc_1, 0, _loc_1.Length)
        End Using
        _loc_2.Position = 0
        Dim _loc_4 As Byte() = New Byte(_loc_2.Length - 1) {}
        _loc_2.Read(_loc_4, 0, _loc_4.Length)
        Return _loc_4
    End Function

    Public Function HW_BytesToStr(param1 As Byte()) As String
        Dim _loc_7 As Byte() = New Byte(19) {}
        For _loc_8 = 0 To 3
            Dim _loc_9 As Long = BitConverter.ToUInt32(New Byte() {param1(_loc_8 * 4), param1(_loc_8 * 4 + 1), param1(_loc_8 * 4 + 2), param1(_loc_8 * 4 + 3)}, 0)
            Dim _loc_10 As Long = 74805201
            For _loc_11 = 4 To 0 Step -1
                _loc_7(_loc_8 * 5 + _loc_11) = Math.Floor(_loc_9 / _loc_10)
                _loc_9 = _loc_9 Mod _loc_10
                _loc_10 /= 93
            Next
        Next
        For _loc_12 = 0 To 19
            If _loc_7(_loc_12) = 30 Then
                _loc_7(_loc_12) = 126
            Else
                _loc_7(_loc_12) += 33
            End If
        Next
        Return Encoding.UTF8.GetString(_loc_7)
    End Function

    ReadOnly HW_CTREE_CRC_Table As UInteger() = {&H0UI, &H4C11DB7UI, &H9823B6EUI, &HD4326D9UI, &H130476DCUI, &H17C56B6BUI, &H1A864DB2UI, &H1E475005UI, &H2608EDB8UI, &H22C9F00FUI, &H2F8AD6D6UI, &H2B4BCB61UI, &H350C9B64UI, &H31CD86D3UI, &H3C8EA00AUI, &H384FBDBDUI, &H4C11DB70UI, &H48D0C6C7UI, &H4593E01EUI, &H4152FDA9UI, &H5F15ADACUI, &H5BD4B01BUI, &H569796C2UI, &H52568B75UI, &H6A1936C8UI, &H6ED82B7FUI, &H639B0DA6UI, &H675A1011UI, &H791D4014UI, &H7DDC5DA3UI, &H709F7B7AUI, &H745E66CDUI, &H9823B6E0UI, &H9CE2AB57UI, &H91A18D8EUI, &H95609039UI, &H8B27C03CUI, &H8FE6DD8BUI, &H82A5FB52UI, &H8664E6E5UI, &HBE2B5B58UI, &HBAEA46EFUI, &HB7A96036UI, &HB3687D81UI, &HAD2F2D84UI, &HA9EE3033UI, &HA4AD16EAUI, &HA06C0B5DUI, &HD4326D90UI, &HD0F37027UI, &HDDB056FEUI, &HD9714B49UI, &HC7361B4CUI, &HC3F706FBUI, &HCEB42022UI, &HCA753D95UI, &HF23A8028UI, &HF6FB9D9FUI, &HFBB8BB46UI, &HFF79A6F1UI, &HE13EF6F4UI, &HE5FFEB43UI, &HE8BCCD9AUI, &HEC7DD02DUI, &H34867077UI, &H30476DC0UI, &H3D044B19UI, &H39C556AEUI, &H278206ABUI, &H23431B1CUI, &H2E003DC5UI, &H2AC12072UI, &H128E9DCFUI, &H164F8078UI, &H1B0CA6A1UI, &H1FCDBB16UI, &H18AEB13UI, &H54BF6A4UI, &H808D07DUI, &HCC9CDCAUI, &H7897AB07UI, &H7C56B6B0UI, &H71159069UI, &H75D48DDEUI, &H6B93DDDBUI, &H6F52C06CUI, &H6211E6B5UI, &H66D0FB02UI, &H5E9F46BFUI, &H5A5E5B08UI, &H571D7DD1UI, &H53DC6066UI, &H4D9B3063UI, &H495A2DD4UI, &H44190B0DUI, &H40D816BAUI, &HACA5C697UI, &HA864DB20UI, &HA527FDF9UI, &HA1E6E04EUI, &HBFA1B04BUI, &HBB60ADFCUI, &HB6238B25UI, &HB2E29692UI, &H8AAD2B2FUI, &H8E6C3698UI, &H832F1041UI, &H87EE0DF6UI, &H99A95DF3UI, &H9D684044UI, &H902B669DUI, &H94EA7B2AUI, &HE0B41DE7UI, &HE4750050UI, &HE9362689UI, &HEDF73B3EUI, &HF3B06B3BUI, &HF771768CUI, &HFA325055UI, &HFEF34DE2UI, &HC6BCF05FUI, &HC27DEDE8UI, &HCF3ECB31UI, &HCBFFD686UI, &HD5B88683UI, &HD1799B34UI, &HDC3ABDEDUI, &HD8FBA05AUI, &H690CE0EEUI, &H6DCDFD59UI, &H608EDB80UI, &H644FC637UI, &H7A089632UI, &H7EC98B85UI, &H738AAD5CUI, &H774BB0EBUI, &H4F040D56UI, &H4BC510E1UI, &H46863638UI, &H42472B8FUI, &H5C007B8AUI, &H58C1663DUI, &H558240E4UI, &H51435D53UI, &H251D3B9EUI, &H21DC2629UI, &H2C9F00F0UI, &H285E1D47UI, &H36194D42UI, &H32D850F5UI, &H3F9B762CUI, &H3B5A6B9BUI, &H315D626UI, &H7D4CB91UI, &HA97ED48UI, &HE56F0FFUI, &H1011A0FAUI, &H14D0BD4DUI, &H19939B94UI, &H1D528623UI, &HF12F560EUI, &HF5EE4BB9UI, &HF8AD6D60UI, &HFC6C70D7UI, &HE22B20D2UI, &HE6EA3D65UI, &HEBA91BBCUI, &HEF68060BUI, &HD727BBB6UI, &HD3E6A601UI, &HDEA580D8UI, &HDA649D6FUI, &HC423CD6AUI, &HC0E2D0DDUI, &HCDA1F604UI, &HC960EBB3UI, &HBD3E8D7EUI, &HB9FF90C9UI, &HB4BCB610UI, &HB07DABA7UI, &HAE3AFBA2UI, &HAAFBE615UI, &HA7B8C0CCUI, &HA379DD7BUI, &H9B3660C6UI, &H9FF77D71UI, &H92B45BA8UI, &H9675461FUI, &H8832161AUI, &H8CF30BADUI, &H81B02D74UI, &H857130C3UI, &H5D8A9099UI, &H594B8D2EUI, &H5408ABF7UI, &H50C9B640UI, &H4E8EE645UI, &H4A4FFBF2UI, &H470CDD2BUI, &H43CDC09CUI, &H7B827D21UI, &H7F436096UI, &H7200464FUI, &H76C15BF8UI, &H68860BFDUI, &H6C47164AUI, &H61043093UI, &H65C52D24UI, &H119B4BE9UI, &H155A565EUI, &H18197087UI, &H1CD86D30UI, &H29F3D35UI, &H65E2082UI, &HB1D065BUI, &HFDC1BECUI, &H3793A651UI, &H3352BBE6UI, &H3E119D3FUI, &H3AD08088UI, &H2497D08DUI, &H2056CD3AUI, &H2D15EBE3UI, &H29D4F654UI, &HC5A92679UI, &HC1683BCEUI, &HCC2B1D17UI, &HC8EA00A0UI, &HD6AD50A5UI, &HD26C4D12UI, &HDF2F6BCBUI, &HDBEE767CUI, &HE3A1CBC1UI, &HE760D676UI, &HEA23F0AFUI, &HEEE2ED18UI, &HF0A5BD1DUI, &HF464A0AAUI, &HF9278673UI, &HFDE69BC4UI, &H89B8FD09UI, &H8D79E0BEUI, &H803AC667UI, &H84FBDBD0UI, &H9ABC8BD5UI, &H9E7D9662UI, &H933EB0BBUI, &H97FFAD0CUI, &HAFB010B1UI, &HAB710D06UI, &HA6322BDFUI, &HA2F33668UI, &HBCB4666DUI, &HB8757BDAUI, &HB5365D03UI, &HB1F740B4UI}

    Public Function HW_CTREE_CRC(param1 As Byte(), param2 As UInteger) As UInteger
        Dim _loc_6 As UInteger = param2
        For _loc_1 As Integer = 0 To param1.Length - 1
            _loc_6 = (param1(_loc_1) Or (_loc_6 << 8)) Xor HW_CTREE_CRC_Table(_loc_6 >> 24)
        Next
        For _loc_4 As Integer = 4 To 1 Step -1
            _loc_6 = HW_CTREE_CRC_Table(BitConverter.GetBytes(_loc_6)(3)) Xor (_loc_6 << 8)
        Next
        Return _loc_6
    End Function

    Public Function HW_CTREE_CRC32(param1 As Byte()) As Byte()
        Dim _loc_1 As UInteger = &H0UI
        Dim _loc_2 As Integer = 0
        While param1.Length - _loc_2 > 1024
            Dim _loc_3 As Byte() = New Byte(1024 - 1) {}
            Array.Copy(param1, _loc_2, _loc_3, 0, 1024)
            _loc_1 = HW_CTREE_CRC(_loc_3, _loc_1)
            _loc_2 += 1024
        End While
        Dim _loc_4 As Byte() = New Byte(param1.Length - _loc_2 - 1) {}
        Array.Copy(param1, _loc_2, _loc_4, 0, param1.Length - _loc_2)
        _loc_1 = HW_CTREE_CRC(_loc_4, _loc_1)
        Return BitConverter.GetBytes(_loc_1)
    End Function

    ReadOnly HW_D2StaticKey As Byte() = HexToBytes("6FC6E3436A53B6310DC09A475494AC774E7AFB21B9E58FC8E58B5660E48E2498")

    Public Function HW_D2Decrypt(param1 As String) As String
        param1 = param1.Substring(2, param1.Length - 3).Replace("&quot;", """").Replace("&amp;", "&").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">")
        Dim _loc_1 As Integer = Math.Floor(param1.Length / 20)
        Dim _loc_2 As New List(Of String)
        For _loc_3 = 0 To _loc_1 - 1
            _loc_2.Add(param1.Substring(20 * _loc_3, 20))
        Next
        Dim _loc_5 As Byte() = New Byte((_loc_2.Count - 1) * 16 - 1) {}
        For _loc_4 = 0 To _loc_2.Count - 2
            Array.Copy(HW_StrToBytes(_loc_2(_loc_4)), 0, _loc_5, _loc_4 * 16, 16)
        Next
        Return Encoding.UTF8.GetString(DecryptAES(_loc_5, HW_D2StaticKey, HW_StrToBytes(_loc_2(_loc_2.Count - 1))))
    End Function

    Public Function HW_D2Encrypt(param1 As String, Optional param2 As Boolean = False) As String
        Dim _loc_1 As New Random
        Dim _loc_2 As Byte() = New Byte(15) {}
        _loc_1.NextBytes(_loc_2)
        Dim _loc_3 As Byte() = EncryptAES(Encoding.UTF8.GetBytes(param1), HW_D2StaticKey, _loc_2)
        Dim _loc_4 As Integer = Math.Floor(_loc_3.Length / 16)
        Dim _loc_5 As String = ""
        For _loc_6 = 0 To _loc_4 - 1
            Dim _loc_7 As Byte() = New Byte(15) {}
            Array.Copy(_loc_3, _loc_6 * 16, _loc_7, 0, 16)
            _loc_5 += HW_BytesToStr(_loc_7)
        Next
        _loc_5 = "$2" & _loc_5 & HW_BytesToStr(_loc_2) & "$"
        If param2 Then
            _loc_5 = _loc_5.Replace("&", "&amp;").Replace("""", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;")
        End If
        Return _loc_5
    End Function

    Public Function HW_StrToBytes(param1 As String) As Byte()
        Dim _loc_5 As Byte() = Encoding.UTF8.GetBytes(param1)
        For _loc_6 = 0 To 19
            If _loc_5(_loc_6) = 126 Then
                _loc_5(_loc_6) = 30
            Else
                _loc_5(_loc_6) -= 33
            End If
        Next
        Dim _loc_7 As Byte() = New Byte(15) {}
        For _loc_8 = 0 To 3
            Dim _loc_9 As Long = 0
            Dim _loc_10 As Long = 1
            For _loc_11 = 0 To 4
                _loc_9 += _loc_10 * _loc_5(_loc_8 * 5 + _loc_11)
                _loc_10 *= 93
            Next
            Dim _loc_12 As Byte() = BitConverter.GetBytes(_loc_9)
            _loc_7(_loc_8 * 4 + 0) = _loc_12(0)
            _loc_7(_loc_8 * 4 + 1) = _loc_12(1)
            _loc_7(_loc_8 * 4 + 2) = _loc_12(2)
            _loc_7(_loc_8 * 4 + 3) = _loc_12(3)
        Next
        Return _loc_7
    End Function

    Public Function HexToBytes(param1 As String) As Byte()
        Return Enumerable.Range(0, param1.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(param1.Substring(x, 2), 16)).ToArray()
    End Function

    Public Function Int32ToBytes(param1 As Integer) As Byte()
        Return BitConverter.GetBytes(param1)
    End Function

    Private Sub ReleaseResource(param1 As String, param2 As String)
        Dim _loc_1 As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream(param1)
        Dim _loc_2(_loc_1.Length - 1) As Byte
        With _loc_1
            .Read(_loc_2, 0, _loc_2.Length)
            .Close()
            .Dispose()
        End With
        Dim _loc_3 As New System.IO.StreamWriter(param2)
        With _loc_3
            .BaseStream.Write(_loc_2, 0, _loc_2.Length)
            .Close()
            .Dispose()
        End With
    End Sub

    ReadOnly HW_CTREE_StaticKey As String = "hex:13395537D2730554A176799F6D56A239"

    Private Sub LoadConfig(param1 As String)
        Try
            ResetBanner()
            LBL_BANNER.Text = "STANDBY"
            Thread.Sleep(100)

            If My.Computer.FileSystem.FileExists(param1) Then
                Dim InputBuffer As Byte() = My.Computer.FileSystem.ReadAllBytes(param1)
                If InputBuffer(0) = &H1 Or InputBuffer(0) = &H2 Then
                    Dim _loc_1 As Byte() = New Byte(3) {}
                    Array.Copy(InputBuffer, 4, _loc_1, 0, 4)
                    Dim PayloadCRC As String = BytesToHex(_loc_1)

                    Dim HeaderLength As Integer = 8
                    Dim DecryptKey As String = HW_CTREE_StaticKey
                    If InputBuffer(0) = &H2 Then
                        Array.Copy(InputBuffer, 8, _loc_1, 0, 4)
                        Dim DecryptKeyLength As Integer = BytesToInt32(_loc_1)
                        Dim DecryptKeyBuffer As Byte() = New Byte(DecryptKeyLength - 1) {}
                        Array.Copy(InputBuffer, 12, DecryptKeyBuffer, 0, DecryptKeyLength)
                        DecryptKey = HW_D2Decrypt(Encoding.UTF8.GetString(DecryptKeyBuffer))
                        HeaderLength = 12 + DecryptKeyLength
                    End If

                    Dim Payload As Byte() = New Byte(InputBuffer.Length - HeaderLength - 1) {}
                    Array.Copy(InputBuffer, HeaderLength, Payload, 0, InputBuffer.Length - HeaderLength)
                    If Not BytesToHex(HW_CTREE_CRC32(Payload)) = PayloadCRC Then
                        ResultFail()
                        Exit Sub
                    End If

                    Dim DecryptBuffer As Byte() = AesCrypt2(Payload, 1, Encoding.UTF8.GetBytes(DecryptKey))
                    If DecryptBuffer(0) = &H1F And DecryptBuffer(1) = &H8B Then
                        Dim UnzipBuffer As Byte() = UnGZip(DecryptBuffer)
                        RenameConfig(param1)
                        My.Computer.FileSystem.WriteAllBytes(param1, UnzipBuffer, False)
                        ResultOK()
                        Exit Sub
                    ElseIf DecryptBuffer.Length > 0 Then
                        RenameConfig(param1)
                        My.Computer.FileSystem.WriteAllBytes(param1, DecryptBuffer, False)
                        ResultOK()
                        Exit Sub
                    Else
                        ResultFail()
                        Exit Sub
                    End If
                ElseIf InputBuffer(0) = &H3 Then
                    RenameConfig(param1)
                    My.Computer.FileSystem.WriteAllBytes(param1, GetCtce8File(InputBuffer), False)
                    ResultOK()
                    Exit Sub
                ElseIf InputBuffer(0) = &H16 And InputBuffer(1) = &H4 And InputBuffer(2) = &H19 And InputBuffer(3) = &H20 And InputBuffer(4) = &H20 And InputBuffer(5) = &H35 And InputBuffer(6) = &H34 And InputBuffer(7) = &H0 Then
                    Dim _loc_2 As String = ""
                    Dim _loc_1 As Byte() = New Byte(3) {}

                    Array.Copy(InputBuffer, 0, _loc_1, 0, 4)
                    _loc_2 &= "Magic A: 0x" & BytesToHex(_loc_1) & vbCrLf

                    Array.Copy(InputBuffer, 4, _loc_1, 0, 4)
                    _loc_2 &= "Magic B: 0x" & BytesToHex(_loc_1) & vbCrLf

                    Array.Copy(InputBuffer, 16, _loc_1, 0, 4)
                    Dim DomainID As Integer = BytesToInt32(_loc_1)
                    _loc_2 &= "Domain ID: " & DomainID & vbCrLf

                    Array.Copy(InputBuffer, 20, _loc_1, 0, 4)
                    Dim KeyID As Integer = BytesToInt32(_loc_1)
                    _loc_2 &= "Key ID: " & KeyID & vbCrLf

                    Array.Copy(InputBuffer, 40, _loc_1, 0, 4)
                    Dim RawDataLen As Integer = BytesToInt32(_loc_1)
                    _loc_2 &= "Plain Data Length: " & RawDataLen & vbCrLf

                    Array.Copy(InputBuffer, 44, _loc_1, 0, 4)
                    Dim EncryptDataOffset As Integer = BytesToInt32(_loc_1)
                    _loc_2 &= "Encrypted Data Offset: " & EncryptDataOffset & vbCrLf

                    Array.Copy(InputBuffer, 48, _loc_1, 0, 4)
                    Dim EncryptDataLen As Integer = BytesToInt32(_loc_1)
                    _loc_2 &= "Encrypted Data Length: " & EncryptDataLen & vbCrLf

                    Array.Copy(InputBuffer, 52, _loc_1, 0, 4)
                    Dim EncryptDataCRC As String = BytesToHex(_loc_1)
                    _loc_2 &= "Encrypted Data CRC: 0x" & EncryptDataCRC

                    Dim EncryptDataIV As Byte() = New Byte(15) {}
                    Array.Copy(InputBuffer, 24, EncryptDataIV, 0, 16)
                    Dim _loc_21 As String = "Encrypted Data IV: 0x" & BytesToHex(EncryptDataIV)

                    Dim _loc_3 As Byte() = New Byte(36 - 1) {}
                    Array.Copy(InputBuffer, 56, _loc_3, 0, 36)
                    Dim _loc_4 As String = Encoding.UTF8.GetString(_loc_3).Replace(Chr(0), "") & vbCrLf & vbCrLf

                    If MsgBox(_loc_4 & _loc_2, vbInformation + vbOKCancel, "版本文件信息") = vbCancel Then
                        ResetBanner()
                        Exit Sub
                    End If

                    Dim KMCFile As String = Path.GetDirectoryName(param1) & "\" & DomainID.ToString() & "_" & KeyID.ToString() & ".key"
                    If Not My.Computer.FileSystem.FileExists(KMCFile) Then
                        ResultFail()
                        Exit Sub
                    End If
                    Dim _loc_6 As Byte() = My.Computer.FileSystem.ReadAllBytes(KMCFile)
                    Dim _loc_7 As Byte() = System.Text.Encoding.UTF8.GetBytes("Df7!ui%s9(lmV1L8")
                    Dim _loc_8 As Byte() = New Byte(_loc_6.Length + _loc_7.Length - 1) {}
                    Array.Copy(_loc_6, 0, _loc_8, 0, _loc_6.Length)
                    Array.Copy(_loc_7, 0, _loc_8, _loc_6.Length, _loc_7.Length)

                    Dim EncryptBuffer As Byte() = New Byte(InputBuffer.Length - EncryptDataOffset - 1) {}
                    Array.Copy(InputBuffer, EncryptDataOffset, EncryptBuffer, 0, EncryptBuffer.Length)
                    Dim DecryptBuffer As Byte() = AesCrypt2(EncryptBuffer, 1, New System.Security.Cryptography.SHA256Managed().ComputeHash(_loc_8))

                    RenameConfig(param1)
                    My.Computer.FileSystem.WriteAllBytes(param1, DecryptBuffer, False)
                    ResultOK()
                    Exit Sub
                ElseIf InputBuffer(0) = &H3C Or Path.GetExtension(param1).ToLower() = ".txt" Then
                    Dim EncryptKey As String = HW_CTREE_StaticKey
                    Dim EncryptMode As MsgBoxResult = MsgBox("使能配置文件动态加密？", vbQuestion + vbYesNoCancel, "选项")
                    If EncryptMode = vbCancel Then
                        ResetBanner()
                        Exit Sub
                    ElseIf EncryptMode = vbYes Then
                        EncryptKey = GetRndString(32)
                    End If

                    If Not Path.GetExtension(param1).ToLower() = ".txt" Then InputBuffer = GZip(InputBuffer)
                    Dim EncryptBuffer As Byte() = AesCrypt2(InputBuffer, 0, Encoding.UTF8.GetBytes(EncryptKey))
                    If EncryptBuffer.Length < 8 Then
                        ResultFail()
                        Exit Sub
                    End If
                    Dim PayloadCRC As Byte() = HW_CTREE_CRC32(EncryptBuffer)

                    Dim OutputBuffer As Byte() = New Byte() {}
                    If EncryptMode = vbYes Then
                        Dim D2EncryptKey As Byte() = Encoding.UTF8.GetBytes(HW_D2Encrypt(EncryptKey))
                        OutputBuffer = New Byte(EncryptBuffer.Length + 12 + D2EncryptKey.Length - 1) {}
                        OutputBuffer(0) = &H2
                        Array.Copy(BitConverter.GetBytes(Convert.ToInt64(D2EncryptKey.Length)), 0, OutputBuffer, 8, 4)
                        Array.Copy(D2EncryptKey, 0, OutputBuffer, 12, D2EncryptKey.Length)
                        Array.Copy(EncryptBuffer, 0, OutputBuffer, 12 + D2EncryptKey.Length, EncryptBuffer.Length)
                    Else
                        OutputBuffer = New Byte(EncryptBuffer.Length + 8 - 1) {}
                        OutputBuffer(0) = &H1
                        Array.Copy(EncryptBuffer, 0, OutputBuffer, 8, EncryptBuffer.Length)
                    End If
                    Array.Copy(PayloadCRC, 0, OutputBuffer, 4, PayloadCRC.Length)

                    RenameConfig(param1)
                    If Path.GetExtension(param1).ToLower() = ".cfg" Then
                        My.Computer.FileSystem.WriteAllBytes(param1, GetCtce8File(OutputBuffer), False)
                    Else
                        My.Computer.FileSystem.WriteAllBytes(param1, OutputBuffer, False)
                    End If
                    ResultOK()
                    Exit Sub
                End If
            End If
            ResultFail()
        Catch ex As Exception
            ResultFail()
        End Try
    End Sub

    Private Sub RenameConfig(param1 As String)
        Dim _loc_1 As Integer = 0
        Dim _loc_2 As String = param1
        While My.Computer.FileSystem.FileExists(_loc_2)
            _loc_1 += 1
            _loc_2 = Path.GetDirectoryName(param1) & "\" & Path.GetFileNameWithoutExtension(param1) & "_" & _loc_1.ToString().PadLeft(4, "0") & Path.GetExtension(param1)
        End While
        My.Computer.FileSystem.MoveFile(param1, _loc_2)
    End Sub

    Private Sub ResetBanner()
        Try
            TMR_BANNER.Enabled = False
            LBL_BANNER.Text = "READY"
            LBL_BANNER.BackColor = Color.White
            LBL_BANNER.ForeColor = Color.Black
            BannerTimer = 0
        Catch ex As Exception

        End Try
    End Sub

    Dim BannerTimer As Integer = 0
    Dim IsResultOK As Boolean = False

    Private Sub ResultFail()
        LBL_BANNER.Text = "FAIL"
        LBL_BANNER.ForeColor = Color.White
        BannerTimer = 0
        IsResultOK = False
        TMR_BANNER.Enabled = True
    End Sub

    Private Sub ResultOK()
        LBL_BANNER.Text = "OK"
        LBL_BANNER.ForeColor = Color.White
        BannerTimer = 0
        IsResultOK = True
        TMR_BANNER.Enabled = True
    End Sub

    Public Function Time() As Long
        Return (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
    End Function

    Public Function UnGZip(param1 As Byte(), Optional param2 As Boolean = True) As Byte()
        Dim _loc_1 As Byte() = param1
        Using _loc_2 As New MemoryStream()
            Dim _loc_3 As Integer = 0
            If param2 Then
                _loc_3 = BitConverter.ToInt32(_loc_1, _loc_1.Length - 4)
                _loc_2.Write(_loc_1, 0, _loc_1.Length - 4)
            Else
                _loc_3 = BitConverter.ToInt32(_loc_1, 0)
                _loc_2.Write(_loc_1, 4, _loc_1.Length - 4)
            End If
            Dim _loc_4 As Byte() = New Byte(_loc_3 - 1) {}
            _loc_2.Position = 0
            Using _loc_5 As New Compression.GZipStream(_loc_2, CompressionMode.Decompress)
                _loc_5.Read(_loc_4, 0, _loc_4.Length)
            End Using
            Return _loc_4
        End Using
    End Function

End Class
