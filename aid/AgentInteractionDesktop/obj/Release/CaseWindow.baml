   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.30, Culture=neutral, PublicKeyToken=null[2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  *    $Agent.Interaction.Desktop.CaseWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  @convert2clr-namespace:Agent.Interaction.Desktop.Converters  6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��UserCaseWindow��  � Title$  
CaseWindow��$
��Auto��$	��550��$
��Auto�� � WindowStyle$
 None=� � 
ResizeMode$ 	CanResize=� � Icon$> 8/Agent.Interaction.Desktop;component/Images/Callinfo.png�� � AllowsTransparency . 
������  � ShowInTaskbar .  � 	Resources  �� MergedDictionaries �� Source9   3Agent.Interaction.Desktop.Converters.ValueConverter   ValueConverterD   >Agent.Interaction.Desktop.Converters.DGCellForeGroundConverter  CellForegroundC   =Agent.Interaction.Desktop.Converters.DGRowBackgroundConverter  RowBackground	 �� 
TargetType  	baseStyle
 �� Property �� Value �� BasedOn  BlinkBorder �� 
TargetName �� TargetProperty W� From	 W� To g� RepeatBehavior g� AutoReverse g� Duration��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%�   &	        &	      &	 
     &	      '�� 2     7  &	 A           �� "	 ��.��� 
 � ��$ 12(��� "	 ��8   �� -   .��W� $ 
MainBorder��$ Opacity�$ 1X�$ 0X�$ 2x�� . $ 0:0:0.1D�.���� -   $��
MainBorder��$��0��$	��0.5q�$��8q�
�������._��  � RowDefinitions �� $
��Auto ��� $��* �.+�� -   $��0��
������� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� ._��� $��5,2,0,0q�$��0��
�������$��60��.+��� -   $��imgSoftIcon��$��15��$��16��$
��Left=�$��Center=�$A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png���� $��1��$��18�����#007edf$��0q�$��Stretch=�$	��Top=�._��� -   $��lblTitleStatus�� �� Padding$ 4,0,4,0q�$��Calibri)�$��13(�
�������$
��Bold"�$��Stretch=����#4DBF4D$��Interaction Data��$
��Left=��� $��2��$��60��$
��Auto��$��Right=�$	��Top=� �� Orientation$ 
Horizontal=�.+��� -   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ �� -   $��btnMaximize��$��16��$��20��  MaximizeButton#��[ �� -	   $��btnExit��$��16��$��20��  CloseButton#��[ �� $��1�����#656d90$��5q�$	��0.5q�._�� -
   $��maniGrid�� �� $
��Auto ��� $��* �.+��� -   $��btnDone��$��45��$��55��$��55��$��0��$��5,2,5,2q�  NormalImageButton#��[ $	��Top=�$
��Left=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $:��4/Agent.Interaction.Desktop;component/Images/Done.png��$��40��$��40���� $��Center=� �� TextAlignment$ Center=�$
��Done��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��DoneHeading��$��	Mark Done��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��DoneContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$>��8Agent can mark done after release the voice interaction.���� -   $��brdData��$��5q�$��1��$
��0.89q�$
��Auto�����#007edf._��  �� -    �Name$ columnUrlSpace��$��* �$	��400���� -   $ columnButtons��$��19 ��� -   $ 
columnData��$	��400��.+�� $��Stretch=�$��Stretch=�.+��� -   $��
imgLoading�� K� ZIndex$ 10000��$��Center=�$��Center=�"  WpfAnimatedGif.ImageBehavior   AnimatedSource$= 7/Agent.Interaction.Desktop;component/Images/Loading.GIF��$��80��$��80��(  "System.Windows.Controls.WebBrowser  -   $��
webBrowser��$y�Hidden=�� $��1��$��2,0,2,0q� �� $
��Auto ��� .+��� -   $��btnShowHideUrlPanel��$��0��#��[ $	��Top=�$	��Top=�.���� $ Vertical=�$��Center=�.+��� -   $��imgShowHideUrlPanel��$?��9/Agent.Interaction.Desktop;component/Images/Show_Left.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��ToolHeading��$
��Hide��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��ToolContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$#��Agent can hide the data panel��A� $��1�� A� Columns$ 1�� A� Rows$ 2��.+�d� -   $��btnData�� � Cursor$
 Hand�� 	 
SideButton#��[	 $��0,0,0,1q�.��� .+��� -   $��imgData��$��18��$��10��$��Right=����� *CasedataImageSource+�� $
��DATA��$��9(�$
��Wrap=�
�������d� -   $��btnContacts��$
 Hand��#��[	 $��0,0,0,1q�.��� .+��� -   $��imgContacts��$��18��$��10��$��Right=����� *ContactImageSource+�� $��CONTACT��$��9(�$
��Wrap=�
��������� -   $��stkDataPanel��$��2��$��0��$��2,0,2,0q�$
��Auto��$ Vertical=�$��Stretch=�.+�� -   $��grdCaseData��$��Stretch=�.+��� -   $��DataTabControl��$��2q�.@��� -   $��tabCaseData��$��Interaction Data��.��� -    $��grdInteractionData�� �� $
��Auto ��� .+��� -!   $��btnAddCallData��$��0��$��30��$��30��$
 1000��$��0,2,5,2q�#��[ $	��Top=�$��Right=�$��Stretch=�y��� *IsVoiceEnabledAddCallData+.���� $ Vertical=�$��Center=�.+��� $9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��15��$��15���� $��Center=�$ Center=�$	��Add��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $	��Add��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Insert addition call data���� -"   $��txtAttachDataInfo��$��1��$��10q�$��No attach data found.��$��Calibri)�$��12(�$��Stretch=�$��Center=�$
��Auto��)  #Microsoft.Windows.Controls.DataGrid  -#   $��DGAttachData��$��1��    RowHeaderWidth$  0Q�
�������$��Stretch=�$	��Top=�$
��Left=�!   HorizontalGridLinesBrush! �#D6D7D6"   VerticalGridLinesBrush" �#D6D7D6$
��Auto=�#   AutoGenerateColumns# .  $   CanUserAddRows$ .  %   CanUserDeleteRows% .  &   CanUserSortColumns& .  ���� *NotifyCallData+' � ContextMenu' �� .@�m� -$   $
��Copy��$��_Copy��( m� InputGestureText$(  ��) m� Command$
) Copy��*   Columns* 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn+   Header$	+ Key��,   Width8	  2Microsoft.Windows.Controls.DataGridLengthConverter$	, 180	 -   MinWidth$	- 150Q�.   MaxWidth$	. 200Q�/   CanUserSort/ .  0 �� ShowOnDisabled0 . 1   
IsReadOnly1 . ���� *
Fontweight+2   
FontFamily2 �� *
Fontfamily+4
  .Microsoft.Windows.Controls.DataGridBoundColumn3 
  Binding3 �� *Key+7  1Microsoft.Windows.Controls.DataGridTemplateColumn  $+ Value��$, *	 4   ClipboardContentBinding4 �� *Value+5   CellTemplate5 �� .R��� $��txtValue��$
��Left=�$��Normal"�$��Calibri)�0 . ���� *Value+6   CellEditingTemplate6 �� .R��� $��	editStack��$ 
Horizontal=�.+��� $��txtValue��7 �� 	UndoLimit$7 10��$
��Left=����� *Value+8 �� Mode$8 TwoWay=��� $��	btnUpdate��-%   $
 Hand��$��5,0,0,0q�$
��Left=�$	��Top=�$
��Auto��$
��Auto�����#007EDF
������ $ Update��.���� $��Update��$��	Underline{����� 9 �� 
TargetType"9 ��.R��� #��z���� $��	btnCancel��-&   $
 Hand��$��5,0,0,0q�$
��Left=�$	��Top=�$
��Auto��$
��Auto�����#007EDF
������ $ Cancel��.���� $��Cancel��$��	Underline{����� "9 ��.R��� #��z��:   	CellStyle: �� -  'Microsoft.Windows.Controls.DataGridCell"	  .��� 
 � ��$ 0q��� 
 � �� �� *Value+; �� 	Converter#; [ < �� Triggers< P� = P� Property= �@ 
IsSelected> P� Value> . . ��� 
 � �� 
 HighlightedBrush# [
 �� 
 � ��
 �   �?   RowStyle? �� ,  &Microsoft.Windows.Controls.DataGridRow"	  .��� 
 � ��$ 0q�< P� = �@ 
IsSelected> . . ��� 
 � ��# [
 �� 
 � ��
 �   ��� -'   $��tabDisposition��$��Disposition Code���� -(   $��tabNotes��$��Notes��.��� $��Stretch=�$��Stretch=� �� $��10��$��* ��� $��40��$
��Auto � �� $��* �.+��� -)   $��txtNotes��$7 10��$��0��$��0��$y�Visible=�$��Stretch=�@ �� AcceptsReturn@ . A �� 
AcceptsTabA . $
��Auto=�$
��Auto=���.   controltempTextbox#��[ $��50��$��2q�$��Normal"�B �� TextWrapping$
B Wrap=��� -*   $��btnSaveNote��$��1��$��0��$��50��$��25��$��Center=�$y�	Collapsed=�$��2,2,5,2q�$��Right=�  NormalButton#��[ $
��Save��$��Normal"�� -+   $��grdContacts��$y�	Collapsed=�$��Stretch=�