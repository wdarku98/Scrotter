﻿'Scrotter, a program designed by yttrium to frame mobile screenshots.
'Copyright (C) 2013 Alex West
'Version 0.8 Public Beta
'
'This program is free software; you can redistribute it and/or
'modify it under the terms of the GNU General Public License
'as published by the Free Software Foundation; either version 2
'of the License, or (at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.
'
'The GNU General Public License may be read at http://www.gnu.org/licenses/gpl-2.0.html.

Imports System.Net
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Threading
Imports System.Environment

Public Class Scrotter
    Public Shared OpenPath(7), SavePath As String
    Public OpenStream As Stream = Nothing
    Public SaveStream As Stream = Nothing
    Public PhoneStream As Stream = Nothing
    Public SaveImg As Image
    Public CanvImg(7) As Image
    Public Image2 As New Bitmap(720, 1280)
    Public Shared IsMono As Boolean
    Public ReadOnly Version As String = "0.8"
    Public ReadOnly ReleaseDate As String = "2013-02-12"
    Private Image(7) As String
    Public AppData As String

    Private Sub LoadBtn_Click(sender As Object, e As EventArgs) Handles LoadBtn.Click
        Dim lastfolderopen As String = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.Title = "Please select your screenshot..."
        openFileDialog1.InitialDirectory = lastfolderopen
        openFileDialog1.Filter = "BMP Files(*.BMP)|*.BMP|PNG Files(*.PNG)|*.PNG|JPG Files(*.JPG)|*.JPG|GIF Files(*.GIF)|*.GIF|All Files(*.*)|*.*"
        openFileDialog1.FilterIndex = 5
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            LoadImage.Image = My.Resources._301
            Try
                OpenStream = openFileDialog1.OpenFile()
                If (OpenStream IsNot Nothing) Then
                    OpenPath(ScreenPicker.Value) = openFileDialog1.FileName
                    ScreenshotBox.Text = OpenPath(ScreenPicker.Value)
                    RefreshLists()
                End If
            Catch Ex As Exception
            Finally
                If (OpenStream IsNot Nothing) Then
                    OpenStream.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub Save(sender As Object, e As EventArgs) Handles SaveBtn.Click
        If ScreenAmountPicker.Value > 1 Then
            Dim number As Integer = 1
            Do While number <= ScreenAmountPicker.Value
                If CanvImg(number) Is Nothing Then CanvImg(number) = New Bitmap(CanvImg(1).Width, CanvImg(1).Height)
                number = number + 1
            Loop
            ArrayPreview.ShowDialog()
            Exit Sub
        End If
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.FileName = "Scrotter_" & DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") & ".png"
        saveFileDialog1.Filter = "BMP Files(*.BMP)|*.BMP|PNG Files(*.PNG)|*.PNG|JPG Files(*.JPG)|*.JPG|All Files(*.*)|*.*" '|GIF Files(*.GIF)|*.GIF"
        saveFileDialog1.FilterIndex = 2
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() = DialogResult.OK Then
            SaveImg = CanvImg(1)
            SaveStream = saveFileDialog1.OpenFile()
            If (SaveStream IsNot Nothing) Then
                SavePath = saveFileDialog1.FileName
                SaveStream.Close()
                RefreshLists()
                Dim Filetype As Integer = saveFileDialog1.FilterIndex
                Dim bm As Bitmap = SaveImg
                If Filetype = 1 Then
                    Dim Image3 As New Bitmap(bm.Width, bm.Height)
                    Dim g As Graphics = Graphics.FromImage(Image3)
                    g.Clear(Color.White)
                    g.DrawImage(bm, New Point(0, 0))
                    g.Dispose()
                    g = Nothing
                    Image3.Save(SavePath, System.Drawing.Imaging.ImageFormat.Bmp)
                ElseIf Filetype = 2 Then
                    bm.Save(SavePath, System.Drawing.Imaging.ImageFormat.Png)
                ElseIf Filetype = 3 Then
                    Dim jgpEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                    Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                    Dim myEncoderParameters As New EncoderParameters(1)
                    Dim myEncoderParameter As New EncoderParameter(myEncoder, 98&)
                    myEncoderParameters.Param(0) = myEncoderParameter
                    Dim Image3 As New Bitmap(bm.Width, bm.Height)
                    Dim g As Graphics = Graphics.FromImage(Image3)
                    g.Clear(Color.White)
                    g.DrawImage(bm, New Point(0, 0))
                    g.Dispose()
                    g = Nothing
                    Image3.Save(SavePath, jgpEncoder, myEncoderParameters)
                    'ElseIf Filetype = 2 Then
                    'Dim Image3 As New Bitmap(bm.Width, bm.Height)
                    'Dim g As Graphics = Graphics.FromImage(Image3)
                    'g.Clear(Color.White)
                    'g.DrawImage(bm, New Point(0, 0))
                    'g.Dispose()
                    'g = Nothing
                    'Image3.Save(SavePath, System.Drawing.Imaging.ImageFormat.Gif)
                End If
            End If
        End If
    End Sub

    Private Function GetEncoder(ByVal format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()

        Dim codec As ImageCodecInfo
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next codec
        Return Nothing
    End Function

    Private Sub RefreshLists() Handles ModelBox.SelectedValueChanged
        StretchCheckbox.Enabled = True
        UnderShadowCheckbox.Enabled = True
        GlossCheckbox.Enabled = True
        ShadowCheckbox.Enabled = True
        VariantBox.Enabled = False
        VariantBox.Items.Clear()
        VariantBox.Text = "Variant"
        Select Case ModelBox.Text
            Case "Samsung Galaxy SIII"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"White", "Blue", "Red", "Brown", "Black"})
                VariantBox.SelectedIndex = 0
            Case "Google Nexus 7", "Google Nexus 10", "Motorola Xoom"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Portrait", "Landscape"})
                VariantBox.SelectedIndex = 0
            Case "HTC One X, HTC One X+"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"White", "Black"})
                VariantBox.SelectedIndex = 0
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "Apple iPhone 4", "Apple iPhone 4S", "Apple iPhone 5", "Apple iPad Mini"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Black", "White"})
                VariantBox.SelectedIndex = 0
            Case "Samsung Galaxy SII, Epic 4G Touch"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Model 1", "Model 2"})
                VariantBox.SelectedIndex = 0
            Case "HTC Desire HD, HTC Inspire 4G", "Samsung Galaxy SIII Mini", "Motorola Droid RAZR", "Motorola Droid RAZR M", "HP TouchPad", "HP Veer", "HTC Evo 3D", "HTC Vivid", "HTC Desire", "Samsung Galaxy Ace, Galaxy Cooper", "Sony Ericsson Xperia J", "LG Nitro HD, Spectrum, Optimus LTE/LTE L-01D/True HD LTE/LTE II", "Samsung Galaxy SII Skyrocket", "HTC Evo 4G LTE", "ASUS Eee Pad Transformer", "HTC Desire C", "LG Optimus 2X", "HTC Wildfire", "HTC Wildfire S", "HTC Amaze 4G, Ruby"
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "HTC One S", "HTC One V", "Google Nexus 4", "HTC Google Nexus One", "HTC Legend", "HTC Droid DNA"
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "Apple iPhone 3G, 3GS"
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
            Case "Sony Ericsson Xperia X10", "Blackberry Z10"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Black", "White"})
                VariantBox.SelectedIndex = 0
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "Samsung Galaxy Note II"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"White", "Gray"})
                VariantBox.SelectedIndex = 0
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "HTC Desire Z, T-Mobile G2", "Samsung Galaxy Tab 10.1", "Motorola Droid 2, Milestone 2"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Portrait", "Landscape"})
                VariantBox.SelectedIndex = 0
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "Samsung Droid Charge, Galaxy S Aviator, Galaxy S Lightray 4G"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Model 1", "Model 2"})
                VariantBox.SelectedIndex = 0
                GlossCheckbox.Enabled = False
                GlossCheckbox.Checked = False
                UnderShadowCheckbox.Enabled = False
                UnderShadowCheckbox.Checked = False
            Case "Nokia Lumia 920"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Red", "Cyan", "Yellow", "Black", "White", "Grey"})
                VariantBox.SelectedIndex = 0
            Case "HTC 8X"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Blue", "Lime", "Red", "Black"})
                VariantBox.SelectedIndex = 0
            Case "HTC 8S"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Blue", "Lime", "Orange", "Black"})
                VariantBox.SelectedIndex = 0
            Case "Nokia N9", "Nokia Lumia 800"
                VariantBox.Enabled = True
                VariantBox.Items.AddRange({"Black", "Cyan", "Magenta", "White"})
                VariantBox.SelectedIndex = 0
        End Select
        RefreshPreview()
    End Sub

    Private Sub RefreshPreview() Handles VariantBox.SelectedValueChanged, ShadowCheckbox.CheckedChanged, GlossCheckbox.CheckedChanged, UnderShadowCheckbox.CheckedChanged, StretchCheckbox.CheckedChanged, ScreenPicker.ValueChanged
        ScreenshotBox.Text = OpenPath(ScreenPicker.Value)
        If ModelBox.Text = "Samsung Galaxy SIII" Then
            Select Case VariantBox.Text
                Case "Black", "Red", "Brown"
                    GlossCheckbox.Enabled = False
                    GlossCheckbox.Checked = False
            End Select
        End If
        If BackgroundDownloader.IsBusy = False Then
            LoadImage.Image = My.Resources._301
            Dim args As ArgumentType = New ArgumentType()
            args.type = 1
            args.var = VariantBox.Text
            args.model = ModelBox.Text
            BackgroundDownloader.RunWorkerAsync(args)
        End If
    End Sub

    Private Sub BackgroundDownloader_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundDownloader.DoWork
        Dim args As ArgumentType = e.Argument
        If args.type = 1 Then
            Dim ScreenCapBitmap As New Bitmap(720, 1280)
            Try
                If String.IsNullOrEmpty(OpenPath(ScreenPicker.Value)) = False Then
                    Image(ScreenPicker.Value) = (OpenPath(ScreenPicker.Value))
                    ScreenCapBitmap = New Bitmap(Image(ScreenPicker.Value))
                End If
            Catch ex As Exception
                MsgBox("Unable to load screenshot.")
                Exit Sub
            End Try
            Dim Image1 As New Bitmap(720, 1280)
            Dim Shadow As New Bitmap(New System.Drawing.Bitmap(New IO.MemoryStream(New System.Net.WebClient().DownloadData("http://ompldr.org/vaDJmbw/1280x720.png"))))
            Dim Gloss As New Bitmap(720, 1280)
            Dim Undershadow As New Bitmap(720, 1280)
            Dim IndexW As Integer = 0
            Dim IndexH As Integer = 0
            Dim Overlay As New Bitmap(720, 1280)
            Dim r240320 As String = "http://ompldr.org/vaDc2ag/240x320.png"
            Dim r320480 As String = "http://ompldr.org/vaGJ2cg/320x480.png"
            Dim r480800 As String = "http://ompldr.org/vaDJmcQ/800x480.png"
            Dim r480854 As String = "http://ompldr.org/vaDQzag/854x480.png"
            Dim r540960 As String = "http://ompldr.org/vaGJ2dA/540x960.png"
            Dim r640960 As String = "http://ompldr.org/vaGJ2dQ/640x960.png"
            Dim r6401136 As String = "http://ompldr.org/vaGJ2dg/640x1136.png"
            Dim r7201280 As String = "http://ompldr.org/vaDJmbw/1280x720.png"
            Dim r7681024 As String = "http://ompldr.org/vaDc3MA/768x1024.png"
            Dim r7681280 As String = "http://ompldr.org/vaGJ2eQ/768x1280.png"
            Dim r800480 As String = "http://ompldr.org/vaDY4Mg/800x480.png"
            Dim r8001280 As String = "http://ompldr.org/vaGJ2eg/800x1280.png"
            Dim r854480 As String = "http://ompldr.org/vaDhxYQ/854x480.png"
            Dim r1024768 As String = "http://ompldr.org/vaDQ1eg/1024x768.png"
            Dim r1280800 As String = "http://ompldr.org/vaGJ3MA/1280x800.png"
            Dim r10801920 As String = "http://ompldr.org/vaDVxdA/1080x1920.png"
            Dim DeviceName As String = ""
            Dim GlossUsed As Boolean = False
            Dim UndershadowUsed As Boolean = False
            Select Case args.model
                Case "Samsung Galaxy SIII Mini"
                    DeviceName = "SamsungGSIIIMini.png"
                    Shadow = FetchImage(r480800)
                    IndexW = 78
                    IndexH = 182
                Case "HTC Desire HD, HTC Inspire 4G"
                    DeviceName = "DesireHD.png"
                    Shadow = FetchImage(r480800)
                    IndexW = 104
                    IndexH = 169
                Case "HTC One X, HTC One X+"
                    If args.var = "Black" Then
                        DeviceName = "OneXBlack.png"
                        IndexW = 113
                    ElseIf args.var = "White" Then
                        DeviceName = "OneXWhite.png"
                        IndexW = 115
                    End If
                    UndershadowUsed = True
                    Shadow = FetchImage(r7201280)
                    IndexH = 213
                Case "Samsung Galaxy SIII"
                    IndexW = 88
                    If args.var = "Blue" Then
                        DeviceName = "GSIIIBlue.png"
                        GlossUsed = True
                    ElseIf args.var = "White" Then
                        DeviceName = "GSIIIWhite.png"
                        GlossUsed = True
                        IndexW = 84
                    ElseIf args.var = "Black" Then
                        DeviceName = "GSIIIBlack.png"
                    ElseIf args.var = "Red" Then
                        DeviceName = "GSIIIRed.png"
                    ElseIf args.var = "Brown" Then
                        DeviceName = "GSIIIBrown.png"
                    End If
                    UndershadowUsed = True
                    Shadow = FetchImage(r7201280)
                    IndexH = 184
                Case "Google Nexus 10"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3Yg/Nexus10Port.png")
                        Shadow = FetchImage(r8001280)
                        Gloss = FetchImage("http://ompldr.org/vaGJ3Yw/Nexus10Port.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3ZA/Nexus10Port.png")
                        IndexW = 217
                        IndexH = 223
                        Dim imgtmp As New Bitmap(800, 1280)
                        Using graphicsHandle As Graphics = Graphics.FromImage(imgtmp)
                            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                            graphicsHandle.DrawImage(ScreenCapBitmap, 0, 0, 800, 1280)
                            ScreenCapBitmap = imgtmp
                        End Using
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3Zw/Nexus10Land.png")
                        Shadow = FetchImage(r1280800)
                        Gloss = FetchImage("http://ompldr.org/vaGJ3ZQ/Nexus10Land.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3Zg/Nexus10Land.png")
                        IndexW = 227
                        IndexH = 217
                        Dim imgtmp As New Bitmap(1280, 800)
                        Using graphicsHandle As Graphics = Graphics.FromImage(imgtmp)
                            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                            graphicsHandle.DrawImage(ScreenCapBitmap, 0, 0, 1280, 800)
                            ScreenCapBitmap = imgtmp
                        End Using
                    End If
                Case "Motorola Xoom"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3ag/XoomPort.png")
                        Shadow = FetchImage(r8001280)
                        Gloss = FetchImage("http://ompldr.org/vaGJ3aA/XoomPort.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3aQ/XoomPort.png")
                        IndexW = 199
                        IndexH = 200
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3aw/XoomLand.png")
                        Shadow = FetchImage(r1280800)
                        Gloss = FetchImage("http://ompldr.org/vaGJ3bA/XoomLand.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3bQ/XoomLand.png")
                        IndexW = 218
                        IndexH = 191
                    End If
                Case "Samsung Galaxy SII, Epic 4G Touch"
                    If args.var = "Model 1" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3bg/GSII.png")
                        Gloss = FetchImage("http://ompldr.org/vaGJ3cA/GSII.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3cQ/GSII.png")
                        IndexH = 191
                    ElseIf args.var = "Model 2" Then
                        DeviceName = FetchImage("http://ompldr.org/vaGJ3cg/Epic4GTouch.png")
                        Gloss = FetchImage("http://ompldr.org/vaGJ3cw/Epic4GTouch.png")
                        Undershadow = FetchImage("http://ompldr.org/vaGJ3dA/Epic4GTouch.png")
                        IndexH = 175
                    End If
                    Shadow = FetchImage(r480800)
                    IndexW = 132
                Case "Apple iPhone"
                    DeviceName = FetchImage("http://102.imagebam.com/download/kwG7EMQO2efuj0oOAGgvAw/23245/232444269/iPhone.png")
                    Shadow = FetchImage(r320480)
                    Gloss = FetchImage("http://104.imagebam.com/download/JYugbZN93ol0aWsjJBZszQ/23245/232446210/iPhone.png")
                    Undershadow = FetchImage("http://101.imagebam.com/download/DQ_mnzd6n6Gm4IT8vRaHPg/23245/232449433/iPhone.png")
                    IndexW = 89
                    IndexH = 176
                Case "Apple iPhone 3G, 3GS"
                    DeviceName = FetchImage("http://108.imagebam.com/download/2fflwcusLtOdHGbKF5bRmA/23245/232444270/iPhone3Gand3GS.png")
                    Shadow = FetchImage(r320480)
                    Undershadow = FetchImage("http://108.imagebam.com/download/rQ-gQf_fK7kTg7i_kaN5Qw/23245/232449438/iPhone3Gand3GS.png")
                    IndexW = 88
                    IndexH = 176
                Case "Apple iPhone 4"
                    If args.var = "Black" Then
                        DeviceName = FetchImage("http://108.imagebam.com/download/krN2_a9Gu7dPs984g8nkQw/23245/232444278/iPhone4Black.png")
                        Gloss = FetchImage("http://108.imagebam.com/download/DB7l2D7aU6lhMs8jKn9u5A/23245/232446214/iPhone4and4SBlack.png")
                    ElseIf args.var = "White" Then
                        DeviceName = FetchImage("http://102.imagebam.com/download/3LTyb6jzu6Dzeni3KoSeNw/23245/232444290/iPhone4White.png")
                        Gloss = FetchImage("http://102.imagebam.com/download/UUZjqzmPlp2jIqu39heYbQ/23245/232446217/iPhone4and4SWhite.png")
                    End If
                    Undershadow = FetchImage("http://104.imagebam.com/download/0H18gXUZG0y653qVMK_zlA/23245/232449444/iPhone4and4S.png")
                    Shadow = FetchImage(r640960)
                    IndexW = 62
                    IndexH = 264
                Case "Apple iPhone 4S"
                    If args.var = "Black" Then
                        DeviceName = FetchImage("http://103.imagebam.com/download/qfbTiEkvju67luFuIII9bA/23245/232444281/iPhone4SBlack.png")
                        Gloss = FetchImage("http://108.imagebam.com/download/DB7l2D7aU6lhMs8jKn9u5A/23245/232446214/iPhone4and4SBlack.png")
                    ElseIf args.var = "White" Then
                        DeviceName = FetchImage("http://103.imagebam.com/download/EM7E87xOKp1u0hCxORtZlQ/23245/232444285/iPhone4SWhite.png")
                        Gloss = FetchImage("http://102.imagebam.com/download/UUZjqzmPlp2jIqu39heYbQ/23245/232446217/iPhone4and4SWhite.png")
                    End If
                    Undershadow = FetchImage("http://104.imagebam.com/download/0H18gXUZG0y653qVMK_zlA/23245/232449444/iPhone4and4S.png")
                    Shadow = FetchImage(r640960)
                    IndexW = 62
                    IndexH = 264
                Case "Apple iPhone 5"
                    If args.var = "Black" Then
                        DeviceName = FetchImage("http://103.imagebam.com/download/Jqa13Vt7YJC7U6h05fmumg/23245/232444294/iPhone5Black.png")
                        Gloss = FetchImage("http://107.imagebam.com/download/-UigK0b5kRfPa4CP74WVTQ/23245/232446218/iPhone5Black-G.png")
                        Undershadow = FetchImage("http://106.imagebam.com/download/cNWALzDFezrQ1B1iGf4GGg/23245/232449448/iPhone5Black-DS.png")
                        Overlay = FetchImage("http://ompldr.org/vaDZhNQ/iPhone5Black.png")
                    ElseIf args.var = "White" Then
                        DeviceName = FetchImage("http://101.imagebam.com/download/ISQSv4cFMh7LrkX8c7EW7A/23245/232444295/iPhone5White.png")
                        Gloss = FetchImage("http://102.imagebam.com/download/6uEehtOHOtHqI8BsnbhU0g/23245/232446221/iPhone5White-G.png")
                        Undershadow = FetchImage("http://108.imagebam.com/download/vDD3AxG4EWHdYi7Hhxz9Cg/23245/232449452/iPhone5White-DS.png")
                        Overlay = FetchImage("http://ompldr.org/vaDZhNg/iPhone5White.png")
                    End If
                    Shadow = FetchImage(r6401136)
                    IndexW = 133
                    IndexH = 287
                Case "Samsung Google Galaxy Nexus"
                    DeviceName = FetchImage("http://107.imagebam.com/download/w1YzISbSAQWkcBcD8d0h9g/23245/232444239/GalaxyNexus.png")
                    Shadow = FetchImage(r7201280)
                    Gloss = FetchImage("http://103.imagebam.com/download/UfQ1I6eQVD4xdv0Pnpgwew/23245/232446201/GalaxyNexus.png")
                    Undershadow = FetchImage("http://103.imagebam.com/download/YIsKjp6AF1sqVkRJkg8Lhw/23245/232449415/GalaxyNexus.png")
                    IndexW = 155
                    IndexH = 263
                Case "Samsung Galaxy Note II"
                    If args.var = "White" Then
                        DeviceName = FetchImage("https://raw.github.com/Yttrium-tYcLief/Scrotter/database/Device/GalaxyNoteII.png")
                    ElseIf args.var = "Gray" Then
                        DeviceName = FetchImage("https://raw.github.com/Yttrium-tYcLief/Scrotter/database/Device/GalaxyNoteIIGray.png")
                    End If
                    IndexW = 49
                    IndexH = 140
                    Shadow = FetchImage(r7201280)
                Case "Motorola Droid RAZR"
                    DeviceName = FetchImage("http://106.imagebam.com/download/hM310SZGxmzR2wxM1IlEOQ/23245/232444231/DroidRAZR.png")
                    Shadow = FetchImage(r540960)
                    IndexW = 150
                    IndexH = 206
                Case "Google Nexus 7"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://104.imagebam.com/download/26ocJdNoE8NTLRhoTR0CDA/23245/232444310/Nexus7Port.png")
                        Shadow = FetchImage(r8001280)
                        Gloss = FetchImage("http://108.imagebam.com/download/Tw_6Jpul1bwHLSfM5ITS6Q/23245/232446227/Nexus7Port.png")
                        Undershadow = FetchImage("http://102.imagebam.com/download/7QKAHQadSzaWxFMpNP8-Jw/23245/232449457/Nexus7Port.png")
                        IndexW = 264
                        IndexH = 311
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://101.imagebam.com/download/Rfj3cR78Rg4So0atGtxjyQ/23245/232444306/Nexus7Land.png")
                        Shadow = FetchImage(r1280800)
                        Gloss = FetchImage("http://101.imagebam.com/download/GPPHiyA4O005EU19Iz4hew/23245/232446226/Nexus7Land.png")
                        Undershadow = FetchImage("http://108.imagebam.com/download/UtdmeHp6BGR_WW4vvM8JUg/23245/232449453/Nexus7Land.png")
                        IndexW = 315
                        IndexH = 270
                    End If
                Case "HTC One S"
                    DeviceName = FetchImage("http://103.imagebam.com/download/pES86Mk-oX3FwKg72ullsg/23245/232444328/OneS.png")
                    Shadow = FetchImage(r540960)
                    Gloss = FetchImage("http://102.imagebam.com/download/2YpfhldGjShokr_7vTVvrA/23245/232446240/OneS.png")
                    IndexW = 106
                    IndexH = 228
                Case "HTC One V"
                    DeviceName = FetchImage("http://103.imagebam.com/download/d78I9T94gLuErZL59eWi6Q/23245/232444333/OneV.png")
                    Shadow = FetchImage(r480800)
                    Gloss = FetchImage("http://101.imagebam.com/download/XztYn-E4j2XfLl8co66zCQ/23245/232446244/OneV.png")
                    IndexW = 85
                    IndexH = 165
                Case "Google Nexus S"
                    DeviceName = FetchImage("http://106.imagebam.com/download/qnwpbb1HFBzATLlQr7yD7g/23245/232444325/NexusS.png")
                    Shadow = FetchImage(r480800)
                    Gloss = FetchImage("http://108.imagebam.com/download/tu5BzK46n3ka_WydBl0pPQ/23245/232446237/NexusS.png")
                    IndexW = 45
                    IndexH = 165
                Case "Google Nexus 4"
                    DeviceName = FetchImage("http://101.imagebam.com/download/fiW5-5yoR6LRtY20rwQmnw/23245/232444302/Nexus4.png")
                    Shadow = FetchImage(r7681280)
                    Gloss = FetchImage("http://104.imagebam.com/download/M_vkC9maazTeEad9DTvD9g/23245/232446224/Nexus4-G.png")
                    IndexW = 45
                    IndexH = 193
                Case "Motorola Droid RAZR M"
                    DeviceName = FetchImage("http://106.imagebam.com/download/E58kNQKNie0lfbXBr8mM-A/23255/232546227/DroidRazrM.png")
                    Shadow = FetchImage(r540960)
                    IndexW = 49
                    IndexH = 129
                Case "Sony Ericsson Xperia X10"
                    If args.var = "Black" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDQzaA/SonyEricssonXperia10Black.png")
                        IndexW = 235
                        IndexH = 191
                    ElseIf args.var = "White" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDQzaQ/SonyEricssonXperia10White.png")
                        IndexW = 255
                        IndexH = 205
                    End If
                    Shadow = FetchImage(r480854)
                Case "HTC Google Nexus One"
                    DeviceName = FetchImage("http://ompldr.org/vaDQzZQ/HTCGoogleNexusOne.png")
                    Shadow = FetchImage(r480800)
                    Gloss = FetchImage("http://ompldr.org/vaDQzOQ/HTCGoogleNexusOne.png")
                    IndexW = 165
                    IndexH = 168
                Case "HTC Hero"
                    DeviceName = FetchImage("http://ompldr.org/vaDQzZg/HTCHero.png")
                    Shadow = FetchImage(r320480)
                    Gloss = FetchImage("http://ompldr.org/vaDQzYQ/HTCHero.png")
                    Undershadow = FetchImage("http://ompldr.org/vaDQzYw/HTCHero.png")
                    IndexW = 67
                    IndexH = 131
                Case "HTC Legend"
                    DeviceName = FetchImage("http://ompldr.org/vaDQzZw/HTCLegend.png")
                    Shadow = FetchImage(r320480)
                    Gloss = FetchImage("http://ompldr.org/vaDQzYg/HTCLegend.png")
                    IndexW = 67
                    IndexH = 131
                    Dim imgtmp As New Bitmap(212, 316)
                    Using graphicsHandle As Graphics = Graphics.FromImage(imgtmp)
                        graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                        graphicsHandle.DrawImage(ScreenCapBitmap, 0, 0, 212, 316)
                        ScreenCapBitmap = imgtmp
                    End Using
                    Dim shdtmp As New Bitmap(212, 316)
                    Using graphicsHandle As Graphics = Graphics.FromImage(shdtmp)
                        graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                        graphicsHandle.DrawImage(Shadow, 0, 0, 212, 316)
                        Shadow = shdtmp
                    End Using
                Case "HP TouchPad"
                    DeviceName = FetchImage("http://ompldr.org/vaDQ1eQ/HPTouchPad.png")
                    Shadow = FetchImage(r1024768)
                    IndexW = 188
                    IndexH = 170
                Case "HP Veer"
                    DeviceName = FetchImage("http://ompldr.org/vaDQ2NQ/HPVeer.png")
                    Shadow = FetchImage("http://ompldr.org/vaDQ2Ng/320x400.png")
                    IndexW = 54
                    IndexH = 77
                    Dim imgtmp As New Bitmap(219, 271)
                    Using graphicsHandle As Graphics = Graphics.FromImage(imgtmp)
                        graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                        graphicsHandle.DrawImage(ScreenCapBitmap, 0, 0, 219, 271)
                        ScreenCapBitmap = imgtmp
                    End Using
                    Dim shdtmp As New Bitmap(219, 271)
                    Using graphicsHandle As Graphics = Graphics.FromImage(shdtmp)
                        graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                        graphicsHandle.DrawImage(Shadow, 0, 0, 219, 271)
                        Shadow = shdtmp
                    End Using
                Case "HTC Droid DNA"
                    DeviceName = FetchImage("http://ompldr.org/vaDVxcQ/DroidDNA.png")
                    Shadow = FetchImage(r10801920)
                    Gloss = FetchImage("http://ompldr.org/vaDY3cw/DroidDNA.png")
                    IndexW = 106
                    IndexH = 300
                Case "HTC Vivid"
                    DeviceName = FetchImage("http://ompldr.org/vaDVxcA/Vivid.png")
                    Shadow = FetchImage(r540960)
                    IndexW = 66
                    IndexH = 125
                Case "HTC Evo 3D"
                    DeviceName = FetchImage("http://ompldr.org/vaDY3dA/Evo3D.png")
                    Shadow = FetchImage(r540960)
                    IndexW = 78
                    IndexH = 153
                Case "HTC Desire Z, T-Mobile G2"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDY4MQ/DesireZPort.png")
                        Shadow = FetchImage(r480800)
                        IndexW = 94
                        IndexH = 162
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDY4MA/DesireZLand.png")
                        Shadow = FetchImage(r800480)
                        IndexW = 189
                        IndexH = 79
                    End If
                Case "HTC Desire"
                    DeviceName = FetchImage("http://ompldr.org/vaDY4Yw/Desire.png")
                    Shadow = FetchImage(r480800)
                    IndexW = 136
                    IndexH = 180
                Case "Samsung Droid Charge, Galaxy S Aviator, Galaxy S Lightray 4G"
                    If args.var = "Model 1" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDY4ZQ/DroidCharge.png")
                        IndexH = 191
                    ElseIf args.var = "Model 2" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDY4bA/GalaxySAviator.png")
                        IndexH = 175
                    End If
                    Shadow = FetchImage(r480800)
                    IndexW = 60
                Case "Samsung Galaxy Ace, Galaxy Cooper"
                    DeviceName = FetchImage("http://ompldr.org/vaDY4cA/GalaxyAce.png")
                    Shadow = FetchImage(r320480)
                    IndexW = 87
                    IndexH = 179
                Case "Nokia Lumia 920"
                    Select Case args.var
                        Case "Red"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5OQ/Lumia920Red.png")
                        Case "Cyan"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5Nw/Lumia920Cyan.png")
                        Case "Yellow"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5Yg/Lumia920Yellow.png")
                        Case "Black"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5Ng/Lumia920Black.png")
                        Case "White"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5YQ/Lumia920White.png")
                        Case "Grey"
                            DeviceName = FetchImage("http://ompldr.org/vaDY5OA/Lumia920Grey.png")
                    End Select
                    Gloss = FetchImage("http://ompldr.org/vaDY5NA/Lumia920.png")
                    Undershadow = FetchImage("http://ompldr.org/vaDY5NQ/Lumia920.png")
                    Shadow = FetchImage(r7681280)
                    IndexW = 160
                    IndexH = 170
                Case "Sony Ericsson Xperia J"
                    DeviceName = FetchImage("http://ompldr.org/vaDY5aQ/XperiaJ.png")
                    Shadow = FetchImage(r480854)
                    IndexW = 75
                    IndexH = 172
                Case "Nokia N9"
                    Select Case args.var
                        Case "Black"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGIyeA/N9BlackGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGIydw/N9Black.png")
                        Case "Cyan"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGIyeg/N9BlueGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGIyeQ/N9Blue.png")
                        Case "Magenta"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGIzMw/N9PinkGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGIzMA/N9Pink.png")
                        Case "White"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGIzNQ/N9WhiteGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGIzNA/N9White.png")
                    End Select
                    Shadow = FetchImage(r480854)
                    Undershadow = FetchImage("http://ompldr.org/vaGIycw/N9.png")
                    IndexW = 83
                    IndexH = 173
                Case "LG Nitro HD, Spectrum, Optimus LTE/LTE L-01D/True HD LTE/LTE II"
                    DeviceName = FetchImage("http://ompldr.org/vaDY5eA/Nitro.png")
                    Shadow = FetchImage(r7201280)
                    IndexW = 113
                    IndexH = 191
                Case "Samsung Galaxy Tab 10.1"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDZhMw/GalaxyTab10.1Port.png")
                        Shadow = FetchImage(r8001280)
                        IndexW = 129
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDZhMg/GalaxyTab10.1Land.png")
                        Shadow = FetchImage(r1280800)
                        IndexW = 135
                    End If
                    IndexH = 135
                Case "Samsung Galaxy SII Skyrocket"
                    DeviceName = FetchImage("http://ompldr.org/vaDZhYw/GSII%20Skyrocket.png")
                    Shadow = FetchImage(r480800)
                    IndexW = 86
                    IndexH = 148
                Case "HTC Evo 4G LTE"
                    DeviceName = FetchImage("http://ompldr.org/vaDZoYw/EVO4GLTE.png")
                    Shadow = FetchImage(r7201280)
                    IndexW = 88
                    IndexH = 199
                Case "ASUS Eee Pad Transformer"
                    DeviceName = FetchImage("http://ompldr.org/vaDZrdw/EeePadTransformer.png")
                    Shadow = FetchImage(r1280800)
                    IndexW = 165
                    IndexH = 157
                Case "HTC 8S"
                    Select Case args.var
                        Case "Blue"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1ZA/8SBlue.png")
                        Case "Lime"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1ZQ/8SGreen.png")
                        Case "Orange"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1Zg/8SOrange.png")
                        Case "Black"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1Yw/8SBlack.png")
                    End Select
                    Gloss = FetchImage("http://ompldr.org/vaDc1dw/8S.png")
                    Undershadow = FetchImage("http://ompldr.org/vaDc1cA/8S.png")
                    Shadow = FetchImage(r480800)
                    IndexW = 130
                    IndexH = 231
                Case "HTC 8X"
                    Select Case args.var
                        Case "Blue"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1aA/8XBlue.png")
                        Case "Lime"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1aQ/8XGreen.png")
                        Case "Red"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1ag/8XRed.png")
                        Case "Black"
                            DeviceName = FetchImage("http://ompldr.org/vaDc1Zw/8XBlack.png")
                    End Select
                    Gloss = FetchImage("http://ompldr.org/vaDc1cg/8X.png")
                    Undershadow = FetchImage("http://ompldr.org/vaDc1cQ/8X.png")
                    Shadow = FetchImage(r7201280)
                    IndexW = 165
                    IndexH = 347
                Case "HTC Desire C"
                    DeviceName = FetchImage("http://ompldr.org/vaDc2NQ/DesireC.png")
                    Shadow = FetchImage(r320480)
                    IndexW = 52
                    IndexH = 101
                Case "HTC Desire C"
                    DeviceName = FetchImage("http://ompldr.org/vaDc2Zw/Wildfire.png")
                    Shadow = FetchImage(r240320)
                    IndexW = 43
                    IndexH = 76
                Case "Apple iPad Mini"
                    If args.var = "Black" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDc2cQ/iPadMiniBlack.png")
                        Overlay = FetchImage("http://ompldr.org/vaDc2cw/iPadMiniBlack.png")
                    ElseIf args.var = "White" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDc2cA/iPadMiniWhite.png")
                        Overlay = FetchImage("http://ompldr.org/vaDc2cg/iPadMiniWhite.png")
                    End If
                    Gloss = FetchImage("http://ompldr.org/vaDc2dQ/iPadMini.png")
                    Undershadow = FetchImage("http://ompldr.org/vaDc2dA/iPadMini.png")
                    Shadow = FetchImage(r7681024)
                    IndexW = 166
                    IndexH = 232
                Case "Motorola Droid 2, Milestone 2"
                    If args.var = "Portrait" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDhxYg/Droid2.png")
                        IndexW = 110
                        IndexH = 193
                        Shadow = FetchImage(r480854)
                    ElseIf args.var = "Landscape" Then
                        DeviceName = FetchImage("http://ompldr.org/vaDhxYw/Droid2Horizontal.png")
                        IndexW = 198
                        IndexH = 95
                        Shadow = FetchImage(r854480)
                    End If
                Case "LG Optimus 2X"
                    DeviceName = FetchImage("http://ompldr.org/vaDhxZA/Optimus2x.png")
                    Shadow = FetchImage(r480800)
                    IndexW = 93
                    IndexH = 175
                Case "HTC Titan"
                    DeviceName = FetchImage("http://ompldr.org/vaGIycg/Titan.png")
                    Shadow = FetchImage(r480800)
                    Gloss = FetchImage("http://ompldr.org/vaGIydA/Titan.png")
                    IndexW = 60
                    IndexH = 138
                Case "HTC Wildfire"
                    DeviceName = FetchImage("http://ompldr.org/vaDc2Zw/Wildfire.png")
                    Shadow = FetchImage(r240320)
                    IndexW = 43
                    IndexH = 76
                Case "HTC Wildfire S"
                    DeviceName = FetchImage("http://ompldr.org/vaGIzNg/WildfireS.png")
                    Shadow = FetchImage(r320480)
                    IndexW = 72
                    IndexH = 123
                Case "HTC Sensation"
                    DeviceName = FetchImage("http://ompldr.org/vaGJxcg/Sensation.png")
                    Shadow = FetchImage(r540960)
                    Gloss = FetchImage("http://ompldr.org/vaGJxcw/Sensation.png")
                    Undershadow = FetchImage("http://ompldr.org/vaGJxdA/Sensation.png")
                    IndexW = 80
                    IndexH = 148
                Case "Nokia Lumia 800"
                    Select Case args.var
                        Case "Black"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGM2aw/Lumia800BlackGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGM2ag/Lumia800Black.png")
                        Case "Cyan"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGM2bQ/Lumia800BlueGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGM2bA/Lumia800Blue.png")
                        Case "Magenta"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGM2bw/Lumia800PinkGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGM2bg/Lumia800Pink.png")
                        Case "White"
                            If GlossCheckbox.Checked Then DeviceName = FetchImage("http://ompldr.org/vaGM2cQ/Lumia800WhiteGloss.png") Else DeviceName = FetchImage("http://ompldr.org/vaGM2cA/Lumia800White.png")
                    End Select
                    Shadow = FetchImage(r480800)
                    Undershadow = FetchImage("http://ompldr.org/vaGM2cg/Lumia800.png")
                    IndexW = 88
                    IndexH = 166
                Case "HTC Amaze 4G, Ruby"
                    DeviceName = FetchImage("http://ompldr.org/vaGNiaw/Ruby.png")
                    Shadow = FetchImage(r540960)
                    IndexW = 84
                    IndexH = 157
            End Select
            Image1 = FetchImage("https://raw.github.com/Yttrium-tYcLief/Scrotter/database/Device/" & DeviceName & ".png")
            If UndershadowUsed = True Then Undershadow = FetchImage("https://raw.github.com/Yttrium-tYcLief/Scrotter/database/Undershadow/" & DeviceName & ".png")
            If GlossUsed = True Then Gloss = FetchImage("https://raw.github.com/Yttrium-tYcLief/Scrotter/database/Gloss/" & DeviceName & ".png")
            If StretchCheckbox.Checked = True Then
                Dim imgtmp2 As New Bitmap(Shadow.Width, Shadow.Height)
                Using graphicsHandle As Graphics = Graphics.FromImage(imgtmp2)
                    graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                    graphicsHandle.DrawImage(ScreenCapBitmap, 0, 0, Shadow.Width, Shadow.Height)
                    ScreenCapBitmap = imgtmp2
                End Using
            End If
            Dim Background As New Bitmap(Image1.Width, Image1.Height)
            Dim Image3 As New Bitmap(Image1.Width, Image1.Height, PixelFormat.Format32bppArgb)
            Dim g As Graphics = Graphics.FromImage(Image3)
            g.Clear(Color.Transparent)
            g.DrawImage(Background, New Point(0, 0))
            If UnderShadowCheckbox.Checked = True Then g.DrawImage(Undershadow, New Point(0, 0))
            g.DrawImage(ScreenCapBitmap, New Point(IndexW, IndexH))
            If ShadowCheckbox.Checked = True Then g.DrawImage(Shadow, New Point(IndexW, IndexH))
            g.DrawImage(Image1, New Point(0, 0))
            If GlossCheckbox.Checked = True Then g.DrawImage(Gloss, New Point(0, 0))
            If (args.model = "Apple iPhone 5") Or (args.model = "Apple iPad Mini") Then g.DrawImage(Overlay, New Point(0, 0))
            g.Dispose()
            g = Nothing
            CanvImg(ScreenPicker.Value) = Image3
        End If
    End Sub

    Public Class ArgumentType
        Public type As Integer
        Public var As String
        Public model As String
    End Class

    Private Function FetchImage(ByVal url As String)
        Try
            Return New Bitmap(New System.Drawing.Bitmap(New IO.MemoryStream(New System.Net.WebClient().DownloadData(url))))
        Catch ex As Exception
        End Try
        Return New Bitmap(720, 1280)
    End Function

    Private Sub BackgroundDownloader_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundDownloader.RunWorkerCompleted
        Preview.Image = CanvImg(ScreenPicker.Value)
        LoadImage.Image = Nothing
    End Sub

    Private Sub CaptureBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CaptureBtn.Click
        adb.ShowDialog()
    End Sub

    Public Shared Sub ADBCapture()
        If Scrotter.IsMono = False Then OpenPath(Scrotter.ScreenPicker.Value) = (Environment.GetEnvironmentVariable("temp") & Path.DirectorySeparatorChar & "capture.png") Else OpenPath(Scrotter.ScreenPicker.Value) = Path.DirectorySeparatorChar & "tmp" & Path.DirectorySeparatorChar & "capture.png"
        Scrotter.RefreshLists()
    End Sub

    Private Sub Scrotter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim t As Type = Type.[GetType]("Mono.Runtime")
        If t Is Nothing Then IsMono = False Else IsMono = True
        about.CheckForUpdates(False)
        If IsMono = False Then AppData = System.IO.Directory.Exists(SpecialFolder.ApplicationData & ".scrotter/") Else System.IO.Directory.Exists(SpecialFolder.Personal & ".scrotter/") 'Per-platform specifics are not usually good as code should be consistent, but this is okay for directory structures
        System.IO.Directory.CreateDirectory(AppData)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        about.ShowDialog()
    End Sub

    Private Sub ScreenAmountPicker_ValueChanged(sender As Object, e As EventArgs) Handles ScreenAmountPicker.ValueChanged
        If ScreenAmountPicker.Value > 1 Then
            ScreenPicker.Maximum = ScreenAmountPicker.Value
            SaveBtn.Text = "Save Multiple Screens As..."
        Else
            ScreenPicker.Value = 1
            ScreenPicker.Maximum = 1
            SaveBtn.Text = "Save As..."
        End If
    End Sub

    Private Sub EnableMultipleScreens(sender As Object, e As EventArgs) Handles ModelBox.TextChanged
        ScreenAmountPicker.Enabled = True
        ScreenPicker.Enabled = True
    End Sub

End Class
