   M S B A M L   `   `   `  ����U  QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=null[2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  5   	   *    $Agent.Interaction.Desktop.CaseWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  5   	   @convert2clr-namespace:Agent.Interaction.Desktop.Converters  5   	   6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   -   5        $��UserCaseWindow��5   Q     � Title$  
CaseWindow��5   	   $
��Auto��6   $	��550��6*   $
��Auto��69    � WindowStyle$
 None=�6F    � 
ResizeMode$ 	CanResize=�6Y    � Icon$> 8/Agent.Interaction.Desktop;component/Images/Callinfo.png��6q    � AllowsTransparency . 
������  � ShowInTaskbar .  � 	Resources 5
       �� MergedDictionaries �� Source9   3Agent.Interaction.Desktop.Converters.ValueConverter   ValueConverterD   >Agent.Interaction.Desktop.Converters.DGCellForeGroundConverter  CellForegroundC   =Agent.Interaction.Desktop.Converters.DGRowBackgroundConverter  RowBackground	 �� 
TargetType  	baseStyle
 �� Property �� Value �� BasedOn  BlinkBorder �� 
TargetName �� TargetProperty W� From	 W� To g� RepeatBehavior g� AutoReverse g� Duration�� 5   
    5      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�6%   %�  &	        &	      &	 &     &	 9     '�� �     7  &	 �       5      6>     5      6I     5      6G   �� 5      "	 ��6   .�5      �� 
 � ��$ 12(�6-   5      �� 5      "	 ��6   8   5      �� 5      -   5        .��5      W� $ 
MainBorder��6"   $ Opacity�5      $ 1X�5      $ 0X�5      $ 2x��5       . $ 0:0:0.1D�5      5      5       .��5"      �� -   5        $��
MainBorder��5"   N   $��0��6   $	��0.5q�6   $��8q�60   
�������._�5#   
   �  � RowDefinitions 5$      �� 5%      $
��Auto �6    60   �� 5&      $��* �6    6-   .+�5(      � -   5        $��0��5(      
������� � ColumnDefinitions 5)      �� 5*      $
��Auto �6'   66   �� 5+      $��* �6'   63   �� 5,      $
��Auto �6'   66   .+�5.      �� ._�5/      �� $��5,2,0,0q�6!   $��0��62   
�������$��60��6U   .+�50      �� -   5        $��imgSoftIcon��50   7   $��15��6    $��16��6+   $
��Left=�6J   $��Center=�6e   $A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png��51   $   52      53      �� 54      $��1��6   $��18��6)   ���#007edf$��0q�6J   $��Stretch=�6U   $	��Top=�6s   ._�55      �� -   5        $��lblTitleStatus��55        �� Padding$ 4,0,4,0q�66   $��Calibri)�6H   $��13(�6]   
�������$
��Bold"�6~   $��Stretch=�6�   ���#4DBF4D$��Interaction Data��56       $
��Left=�69   6V   58      �� 59      $��2��6   $��60��6-   $
��Auto��68   $��Right=�6F   $	��Top=�6b    �� Orientation$ 
Horizontal=�6z   .+�5:      �� -   5        $��btnMinimize��5:      $��16��60   $��20��6<     MinimizeButton#��[ 6G   6o   �� 5;      -   5        $��btnMaximize��5;      $��16��61   $��20��6=     MaximizeButton#��[ 6H   6p   �� 5<      -	   5        $��btnExit��5<      $��16��6,   $��20��68     CloseButton#��[ 6C   6h   5=      5>      �� 5?      $��1��6   ���#656d90$��5q�69   $	��0.5q�6D   ._�5@      � -
   5        $��maniGrid��5@       5A      �� 5B      $
��Auto �6(   68   �� 5C      $��* �6(   65   .+�5U      �� -   5        $��btnDone��5U      $��45��6,   $��55��6:   $��55��6H   $��0��6T   $��5,2,5,2q�6a     NormalImageButton#��[ 6r   $	��Top=�5V   !   $
��Left=�69   $��Stretch=�6T   .��5W      �� $ Vertical=�6%   $��Center=�6<   .+�5X      �� $:��4/Agent.Interaction.Desktop;component/Images/Done.png��6$   $��40��6b   $��40��6m   6{   �� 5Y      $��Center=�6(    �� TextAlignment$ Center=�6E   $
��Done��6]   $��Calibri)�6i   $��11(�6~   $
��Wrap=�6�   $��2,0,2,0q�6�   5Z      5[       � ToolTip 5\      �� 5]      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�5^   "   �  5_   &   �� 5`   *   $
��Auto �68   6H   �� 5a   *   $��* �68   6E   .+�5c   &   �� -   5        $��DoneHeading��5c   0   $��	Mark Done��6C   $��0��6T   $ 	10,5,10,0q�6a   $
��Bold"�6u   $��Calibri)�6�   6�   �� 5d   &   -   5        $��DoneContent��5d   0   $��1��6C   $ 
10,2,10,10q�6P   $��WrapWithOverflow=�6e   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $>��8Agent can mark done after release the voice interaction.��6�   5e   #   5f      5h      �� 5j      -   5        $��brdData��5j      $��5q�6,   $��1��67   $
��0.89q�6D   $
��Auto��6[   ���#007edf._�5k      �  5l      �� 5m   "   -   5         �Name$ columnUrlSpace��5m   3   $��* �6I   $	��400��6S   6d   �� 5n   "   -   5        $ columnButtons��5n   3   $��19 �6H   6U   �� 5o   "   -   5        $ 
columnData��5o   3   $	��400��6E   6V   .+�5r      � $��Stretch=�6#   $��Stretch=�6?   .+�5s   "   �� -   5        $��
imgLoading��5s   (    K� ZIndex$ 10000��6:   $��Center=�6P   $��Center=�6k   "  WpfAnimatedGif.ImageBehavior   AnimatedSource$= 7/Agent.Interaction.Desktop;component/Images/Loading.GIF��6�   $��80��6�   $��80��6�   (  "System.Windows.Controls.WebBrowser  5v   "   -   5        $��
webBrowser��5v   -   $y�Hidden=�6@   6T   5{      � 5|      $��1��6#   $��2,0,2,0q�63    5}   "   �� 5~   &   $
��Auto �64   6D   �� 5   &   66   .+�5�   "   �� -   5        $��btnShowHideUrlPanel��5�   6   $��0��6)   #��[ 6Q   $	��Top=�6|   $	��Top=�5�   -   .��5�   &   �� $ Vertical=�61   $��Center=�6H   .+�5�   *   �� -   5        $��imgShowHideUrlPanel��5�   s   $?��9/Agent.Interaction.Desktop;component/Images/Show_Left.png��60   6�   5�   '    5�   &   �� 5�   *   
����������#ADAAAD$	��0.5q�6Z   $��-5q�6p   ._�5�   .   �  5�   2   �� 5�   6   $
��Auto �6D   6T   �� 5�   6   $��* �6D   6Q   .+�5�   2   �� -   5        $��ToolHeading��5�   <   $
��Hide��6O   $��0��6[   $ 	10,5,10,0q�6h   $
��Bold"�6|   $��Calibri)�6�   6�   �� 5�   2   -   5        $��ToolContent��5�   <   $��1��6O   $ 
10,2,10,10q�6\   $��WrapWithOverflow=�6q   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $#��Agent can hide the data panel��6�   5�   /   5�   +   5�   #   A� 5�   "   $��1��6.    A� Columns$ 1��6;    A� Rows$ 2��6G   .+�5�   &   d� -   5        $��btnData��5�   A    � Cursor$
 Hand��63    	 
SideButton#��[	 6P   $��0,0,0,1q�6t   .��5�   *   � .+�5�   .   �� -   5        $��imgData��5�   4   $��18��6j   $��10��6v   $��Right=�6�   ��6C   �� *CasedataImageSource+6�   �� 5�   .   $
��DATA��68   $��9(�6D   $
��Wrap=�6Q   
�������6z   5�   +   5�   '   d� 5�   &   -   5        $��btnContacts��5�   A   $
 Hand��63   #��[	 6T   $��0,0,0,1q�6x   .��5�   *   � .+�5�   .   �� -   5        $��imgContacts��5�   4   $��18��6m   $��10��6y   $��Right=�6�   ��6G   �� *ContactImageSource+6�   �� 5�   .   $��CONTACT��68   $��9(�6G   $
��Wrap=�6T   
�������6}   5�   +   5�   '   5�   #   5�      �� 5�      -   5        $��stkDataPanel��5�   )   $��2��6=   $��0��6M   $��2,0,2,0q�6Z   $
��Auto��6k   $ Vertical=�6y   $��Stretch=�6�   .+�5�   "   � -   5        $��grdCaseData��5�   '   $��Stretch=�6:   .+�5�   &   �� -   5        $��DataTabControl��5�   1   $��2q�6G   .@�5�   *   �� -   5        $��tabCaseData��5�   L   $��Interaction Data��62   .��5�   .   � -    5        $��grdInteractionData��5�   3    5�   2   �� 5�   6   $
��Auto �6D   �� 5�   6   .+�5�   2   �� -!   5        $��btnAddCallData��5�   9   $��0��6O   $��30��6\   $��30��6g   $
 1000��6s   $��0,2,5,2q�6�   #��[ 6�   $	��Top=�5�   j   $��Right=�6�   $��Stretch=�5�   9   y�5�   9   �� *IsVoiceEnabledAddCallData+.��5�   6   �� $ Vertical=�6A   $��Center=�6X   .+�5�   :   �� $9��3/Agent.Interaction.Desktop;component/Images/Add.png��6@   $��15��6}   $��15��6�   6�   �� 5�   :   $��Center=�6D   $ Center=�6a   $	��Add��6y   $��Calibri)�6�   $��11(�6�   $
��Wrap=�6�   $��2,0,2,0q�6�   5�   7    5�   6   �� 5�   :   
����������#ADAAAD$	��0.5q�6j   $��-5q�6�   ._�5�   >   �  5�   B   �� 5�   F   $
��Auto �6T   6d   �� 5�   F   $��* �6T   6a   .+�5�   B   �� $	��Add��6M   $��0��6X   $ 	10,5,10,0q�6e   $
��Bold"�6y   $��Calibri)�6�   6�   �� 5�   B   $��1��6L   $ 
10,2,10,10q�6Y   $��WrapWithOverflow=�6n   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $��Insert addition call data��6�   5�   ?   5�   ;   5�   3   �� 5�   2   -"   5        $��txtAttachDataInfo��5�   I   $��1��6<   $��10q�6b   $��No attach data found.��6n   $��Calibri)�6�   $��12(�6�   $��Stretch=�5�   5   $��Center=�6Q   $
��Auto��6n   )  #Microsoft.Windows.Controls.DataGrid  5�   2   -#   5        $��DGAttachData��5�   �   $��1��6>       RowHeaderWidth$  0Q�6K   
�������$��Stretch=�6r   $	��Top=�6�   $
��Left=�6�   !   HorizontalGridLinesBrush! �#D6D7D6"   VerticalGridLinesBrush" �#D6D7D6$
��Auto=�5�   N   #   AutoGenerateColumns# .  $   CanUserAddRows$ .  %   CanUserDeleteRows% .  &   CanUserSortColumns& .  ��5�   �   �� *NotifyCallData+' � ContextMenu' 5�   6   �� 5�   :   .@�5�   >   m� -$   5        $
��Copy��5�   V   $��_Copy��6G   ( m� InputGestureText$(  ��6b   ) m� Command$
) Copy��6v   5�   ;   *   Columns* 5�   6   3  -Microsoft.Windows.Controls.DataGridTextColumn  5�   :   /  )Microsoft.Windows.Controls.DataGridColumn+   Header$	+ Key��6P   ,   Width8	  2Microsoft.Windows.Controls.DataGridLengthConverter$	, 180	 6�   -   MinWidth$	- 150Q�6�   .   MaxWidth$	. 200Q�6�   /   CanUserSort/ .  0 �� ShowOnDisabled0 . 1   
IsReadOnly1 . ��6]   �� *
Fontweight+2   
FontFamily2 6�   �� *
Fontfamily+4
  .Microsoft.Windows.Controls.DataGridBoundColumn3 
  Binding3 6�   �� *Key+6+  7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5�   :   $+ Value��6T   $, *	 6c   4   ClipboardContentBinding4 6m   �� *Value+5   CellTemplate5 5�   >   �� 5�   B   .R�5�   F   �� $��txtValue��6�   $
��Left=�6P   $��Normal"�6k   $��Calibri)�6   0 . ��6�   �� *Value+5�   G   5�   C   6   CellEditingTemplate6 5�   >   �� 5�   B   .R�5�   F   �� $��	editStack��6j   $ 
Horizontal=�6Q   .+�5�   J   �� $��txtValue��6|   7 �� 	UndoLimit$7 10��6R   $
��Left=�6a   ��6�   �� *Value+8 �� Mode$8 TwoWay=��� 5�   J   $��	btnUpdate��6�   -%   5        $
 Hand��5�   R   $��5,0,0,0q�6`   $
��Left=�6r   $	��Top=�6�   $
��Auto��6�   $
��Auto��6�   ���#007EDF
������ $ Update��6  .��5�   N   �� $��Update��6X   $��	Underline{�6f   6�   ��5�   N   �� 5�   R   9 �� 
TargetType"9 ��6b   .R�5�   V   �� #��z��6g   5�   S   5�   K   �� 5�   J   $��	btnCancel��6�   -&   5        $
 Hand��5�   R   $��5,0,0,0q�6`   $
��Left=�6r   $	��Top=�6�   $
��Auto��6�   $
��Auto��6�   ���#007EDF
������ $ Cancel��6  .��5�   N   �� $��Cancel��6X   $��	Underline{�6f   6�   ��5�   N   �� 5�   R   "9 ��6b   .R�5�   V   �� #��z��6g   5�   S   5�   K   5�   G   5�   C   :   	CellStyle: 5�   >   �� 5�   B   -  'Microsoft.Windows.Controls.DataGridCell"	  6H   .�5�   F   �� 
 � ��$ 0q�6h   �� 5�   F   
 � �� 6c   �� *Value+; �� 	Converter#; [ < �� Triggers< 5�   F   P� 5�   J   = P� Property= �@ 
IsSelected> P� Value> . . �5�   N   �� 
 � �� 
 HighlightedBrush# [
 6k   �� 5�   N   
 � ��
 �   �6k   5�   K   5�   C   5�   ;   ?   RowStyle? 5�   6   �� 5   :   ,  &Microsoft.Windows.Controls.DataGridRow"	  6@   .�5  >   �� 
 � ��$ 0q�6`   < 5  >   P� 5  B   = �@ 
IsSelected> . . �5  F   �� 
 � ��# [
 6c   �� 5  F   
 � ��
 �   �6c   5  C   5	  ;   5  3   5  /   5  +   �� 5  *   -'   5        $��tabDisposition��5  L   $��Disposition Code��62   5  +   �� 5  *   -(   5        $��tabNotes��5  A   $��Notes��62   .��5  .   � $��Stretch=�63   $��Stretch=�6O    5  2   �� 5  6   $��10��6D   $��* �6S   6`   �� 5  6   $��40��6D   $
��Auto �6S   6c    5  2   �� 5  6   $��* �6G   6S   .+�5  2   �� -)   5        $��txtNotes��5  J   $7 10��6:   $��0��6Z   $��0��6g   $y�Visible=�6w   $��Stretch=�6�   @ �� AcceptsReturn@ . A �� 
AcceptsTabA . $
��Auto=�5  M   $
��Auto=�6   ��.   controltempTextbox#��[ 5  `   $��50��6�   $��2q�6�   $��Normal"�5  ;   B �� TextWrapping$
B Wrap=�6O   6�   �� 5   2   -*   5        $��btnSaveNote��5   9   $��1��6L   $��0��6Y   $��50��6i   $��25��6t   $��Center=�6�   $y�	Collapsed=�6�   $��2,2,5,2q�5!  9   $��Right=�6J     NormalButton#��[ 6f   $
��Save��6�   $��Normal"�6�   5$  3   5%  /   5&  +   5(  '   5)  #   � 5*  "   -+   5        $��grdContacts��5*  '   $y�	Collapsed=�6:   $��Stretch=�6Q   5,  #   5-     5.     5/     50     51     52     53     54     