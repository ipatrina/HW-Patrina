<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainUI
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainUI))
        Me.LBL_BANNER = New System.Windows.Forms.Label()
        Me.BTN_BROWSE = New System.Windows.Forms.Button()
        Me.OFD_CONFIG = New System.Windows.Forms.OpenFileDialog()
        Me.TMR_BANNER = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'LBL_BANNER
        '
        Me.LBL_BANNER.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LBL_BANNER.Location = New System.Drawing.Point(14, 14)
        Me.LBL_BANNER.Name = "LBL_BANNER"
        Me.LBL_BANNER.Size = New System.Drawing.Size(120, 32)
        Me.LBL_BANNER.TabIndex = 101
        Me.LBL_BANNER.Text = "READY"
        Me.LBL_BANNER.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BTN_BROWSE
        '
        Me.BTN_BROWSE.Location = New System.Drawing.Point(140, 14)
        Me.BTN_BROWSE.Name = "BTN_BROWSE"
        Me.BTN_BROWSE.Size = New System.Drawing.Size(32, 32)
        Me.BTN_BROWSE.TabIndex = 201
        Me.BTN_BROWSE.Text = "..."
        Me.BTN_BROWSE.UseVisualStyleBackColor = True
        '
        'OFD_CONFIG
        '
        Me.OFD_CONFIG.Filter = "所有文件|*.*"
        '
        'TMR_BANNER
        '
        Me.TMR_BANNER.Interval = 300
        '
        'MainUI
        '
        Me.AcceptButton = Me.BTN_BROWSE
        Me.AllowDrop = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(184, 61)
        Me.Controls.Add(Me.LBL_BANNER)
        Me.Controls.Add(Me.BTN_BROWSE)
        Me.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainUI"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "HW Patrina"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LBL_BANNER As Label
    Friend WithEvents BTN_BROWSE As Button
    Friend WithEvents OFD_CONFIG As OpenFileDialog
    Friend WithEvents TMR_BANNER As Timer
End Class
