   M S B A M L   `   `   `  ����W  SPointel.Interactions.Contact, Version=5.0.3.8, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Pointel.Interactions.Contact.Converters'Pointel.Interactions.Contact.Converters  5      >    8Pointel.Interactions.Contact.Controls.ContactInformation   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      E	Converter5clr-namespace:Pointel.Interactions.Contact.Converters  5      -   5        $��Calibri)�5	      $��12(�6$     � 	Resources  5       �� MergedDictionaries �� SourceG   APointel.Interactions.Contact.Converters.NameToVisibilityConverter   Key  MediaTemplateVoice  MediaTemplateEmail  MediaTemplateChat�� 5   
    5      �� 5      $T N/Pointel.Interactions.Contact;component/Resource/StylesResourceDictionary.xaml?�5      �� 5        5      NameToVisibilityConverter  6:   5      %�  &	       &	 �     &	 8    �� 5      
VoiceMedia��.R�5      �� $��16��6   $��16��6#   $C��=/Agent.Interaction.Desktop;component/Images/Contact/Voice.png��6/   5      �� 5      
EmailMedia��.R�5      �� $��16��6   $��16��6#   $C��=/Agent.Interaction.Desktop;component/Images/Contact/Email.png��6/   5      �� 5      	ChatMedia��.R�5      �� $��16��6   $��16��6#   $B��</Agent.Interaction.Desktop;component/Images/Contact/Chat.png��6/   5      5      .��5      �� $
��Auto��6   ���#D6D7D6$��1q�61   ._�5    
   � .+�5!      �� -   5        $��stk_Options��5!      $��5q�6,    K� ZIndex$
 1000��67    �� Orientation$ 
Horizontal=�6K   $��Right=�6d   $	��Top=�6�   $��25��6�   .+�5"      �� -   5        $��btnSave��5"      $��22��6(   $��22��63   $��0,0,5,0q�6?     NormalImageButton#��[ 6P   $	��Top=�6{   $��Stretch=�6�   .��5#      �� -   5        $��imgSave��5#      $	��0.5q�6+   ��5$      �� 5%       �� 
TargetType" ��6$   .�5&   "   ��  �� Property � �� �� Value$E ?/Agent.Interaction.Desktop;component/Images/Contact/Save-01.png��6;    �� Triggers 5'   "   �� 5(   &   	 �� Binding	 62   �� 
 �� ElementName$
 btnSave�� �� Path$ 	IsEnabled� �� Value	 False6i   .a�5)   *   ��  � ��$K E/Agent.Interaction.Desktop;component/Images/Contact/Save-Disabled.png��6C   5*   '   5,      5.       � ToolTip 5/      �� 50      
����������#ADAAAD$	��0.5q�6J   $��-5q�6`   ._�51      �  � RowDefinitions 52   "   �� 53   &   $
��Auto �64   6D   �� 54   &   $��* �64   6A   .+�56   "   �� -   5        $��SaveHeading��56   ,   $��Save Contact��6?   $��0��6S    �� Padding$ 	10,5,10,0q�6`   $
��Bold"�6t   $��Calibri)�6�   6�   �� 57   "   -   5        $��SaveContent��57   ,   $��1��6?   $ 
10,2,10,10q�6L   $��WrapWithOverflow=�6a   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $#��Save the contact information.��6�   58      59      5;      �� 5<      -   5        $��btnReset��5<      $��22��6)   $��22��64   #��[ 6@   $	��Top=�6k   $��Stretch=�6�   .��5=      �� -   5        $��imgReset��5=      $	��0.5q�6,   ��5>      �� 5?      " ��6$   .�5@   "   ��  � ��$F @/Agent.Interaction.Desktop;component/Images/Contact/Reset-01.png��6;   6�    5A   "   �� 5B   &   	 62   �� $
 btnReset��$ 	IsEnabled�	 False6j   .a�5C   *   ��  � ��$L F/Agent.Interaction.Desktop;component/Images/Contact/Reset-Disabled.png��6C   6�   5D   '   5F      5I       5J      �� 5K      
����������#ADAAAD$	��0.5q�6J   $��-5q�6`   ._�5L      �  5M   "   �� 5N   &   $
��Auto �64   6D   �� 5O   &   $��* �64   6A   .+�5Q   "   �� -	   5        $��ResetHeading��5Q   ,   $��Reset Contact��6@   $��0��6U   $ 	10,5,10,0q�6b   $
��Bold"�6v   $��Calibri)�6�   6�   �� 5R   "   -
   5        $��ResetContent��5R   ,   $$��Reset the contact information.��6@   $��1��6f   $ 
10,2,10,10q�6s   $��WrapWithOverflow=�6�   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   5S      5T      5V      5W      �� 5X      -   5        $��
tbCallBack��5X   6   $��2q�6    �� SelectedIndex$ 0��6$   
�������.@�5Y      �� -   5        $��tabitemInformation��5Y   .   
�������$��Information��6H    �� 
IsSelected . .��5Z      � -   5        $��grdMain��5Z   4   
������  5[      �� 5\      -   5         �Name$ rowDockError��5\   -   $��30 �6A   6O   �� 5]      $��* �6,   6:   .+�5_      � $
��Auto��6   $
��Auto��6,   $��0��6;   
������� � ColumnDefinitions 5`      �� 5a   "   $��30 �63   6@   �� 5b   "   -   5        $ grdErrorColumn��5b   =   $��* �63   6U   �� 5c   "   $��30 �63   6@   .+�5e      �� $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6$   $��0��5f   "   $��5q�63   $��20��6>   $��20��6I   6W   �� 5h      -   5        $��lblError��5h   (   $��1��68   $
��Wrap=�6a   $��Center=�6v   $��5q�6�   ;9Can't save this contact , Enter the fields before saving.5j      �� 5m      -   5        $��btnCloseError��5m   %   $��Right=�6:   $��2��6V   $��20��6f   $��20��6r   #��[ 6~   $��Center=�6�   $��4q�6�   .��5n   "   �� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��6(   6v    5o   "   �� 5p   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5q   *   �  5r   .   �� 5s   2   $
��Auto �6@   6P   �� 5t   2   $��* �6@   6M   .+�5v   .   �� $��Close Error��68   $��0��6K   $ 	10,5,10,0q�6X   $
��Bold"�6l   $��Calibri)�6~   6�   �� 5w   .   $��Close the error messages.��68   $��1��6Y   $ 
10,2,10,10q�6f   $��WrapWithOverflow=�6{   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   5x   +   5y   '   5{      5|      �� 5}      -   5        $��scv_Information��5}   '   $��2,4,2,2q�6>   $��1��6O   $��Stretch=�6\   $
��Auto=�6x   $
��Auto=�6�     5~       P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%  '��       7  7  �� 5   "   " ��6C   .�5�   &   ��  �@J�SnapsToDevicePixels . 6L   �� 5�   &    �@�OverridesDefaultStyle . 6N    5�   &   P� 5�   *    �@��Orientation$ 
Horizontal=�6I   . �5�   .   ��  � ��$
 Auto��6F   �� 5�   .    � ��$ 18��6G   �� 5�   .    � ��8   6I   5�   +   P� 5�   *    �@��Orientation$ Vertical=�6I   . �5�   .   ��  � ��$ 18��6F   �� 5�   .    � ��$
 Auto��6G   �� 5�   .    � ��8  6I   5�   +   5�   #   5�      .��5�      �� -   5        $��
stkPnlInfo��5�   )   $ Vertical=�6;   .+�5�   "   �� $��8q�6+   $��General��66   $
��Auto��6G   ���#ADAAAD$	��0.5q�6j   .��5�   &   )  #Microsoft.Windows.Controls.DataGrid  -   5        $��dgGeneralInfo��5�   2      GridLinesVisibility$
 None=�6G   
������ $��0q�6|   
������    AutoGenerateColumns .     CanUserAddRows .     HeadersVisibility$
 None=�5�   \   |�.     Columns 5�   *   7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5�   .   /  )Microsoft.Windows.Controls.DataGridColumn   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	 120 6H      
IsReadOnly .    CellTemplate 5�   2   �� 5�   6   .R�5�   :   �� $ 
Horizontal=�6E   $	��100��6^   .+�5�   >   �� |�.  $��4,4,0,4q�6y   ��6H   �� *AttributeName+6y   �� 5�   >   $�� *��6H   $��0,2,4,0q�6R   
���  ��|�.  $��Calibri)�6�   $��14(�6�   y�6�   �� *AttributeName+  NameToVisibilityConverter �� 	Converter# [ 5�   ;   5�   7      	CellStyle 5�   2   �� 5�   6   -  'Microsoft.Windows.Controls.DataGridCell"  6<   .�5�   :   ��  � �� .  6f    5�   :   P� 5�   >    �@ 
IsSelected . . �5�   B   ��  � ��
 ���� 6u   �� 5�   B    � ��
 �   �6o   �� 5�   B    � ��$ 0q�6d   6p   5�   ?   5�   7   5�   /     5�   .   $	 120 6H    5�   2   �� 5�   6   .R�5�   :   �� -   5          �� 	UndoLimit$  10��5�   B   $��120px��6�   $��26��6�     DefaultCopyContextmenu! � ContextMenu#! [ 6�    	 MyWaterMarkStyle#��[	 5�   U   $��0,4,0,4q�6   $��Center=�6�   ��5�   Q   �� *AttributeValue+
" � Tag" 5�   7   �� *AttributeName+6�   5�   7   5�   /     5�   .    5�   2   �� 5�   6   .R�5�   :   �� $��btnDeleteSingleAtt��6A   -   5        $��11��5�   \   $��11��6h   $��3,6,0,6q�6s   #��[ 6�   $��Center=�6�   .��5�   >   �� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��6D   6�   5�   ;   5�   7   5�   /   5�   '   5�   #     5�   "   -   5        $��dgGroupData��5�   .   $
 None=�6A   
������ 
������  .   .  $
 None=�5�   a   # +� FocusedElement# 5�   Y   �� $
 txtAttributeValue��  5�   &    
 	GroupData$ �� 
TargetType  NormalButton��%�  &	
       7  7  �� 5�   *   " ��6B   .�5�   .   ��  � �� 5�   2   �� 5�   6   "$ ��6F   .R�5�   :   �� $��10q�6D   ��5�   >   �� 5�   B   $ 
Horizontal=�6M   .+�5�   F   �� ��6P   �� *Name+�� 5�   F   $�� *��6P   
���  ��$��Calibri)�6k   $��14(�6�   y�6�   �� *Name+8   5�   C   .��5�   >   �� $ Vertical=�6I   .+�5�   B   �� 6S   �� 5�   B   $��btnAdd��6I   -   5        $��23��5�   W   $
��Left=�6c   $	��180��6�   $��4q�6�   $��5,0,5,0q�6�   8�� .��5�   F   �� $ 
Horizontal=�6Q   .+�5�   J   �� $
��Add ��6T   
�������6v   �� 5�   J   
���������6U   �� *Name+6�   5�   G   5�   C   5�   ?   5�   ;   5�   7   5�   /   5�   +   5�   '   % �� 
GroupStyle% 5�   &   (  "System.Windows.Controls.GroupStyle  5�   *   &   ContainerStyle#& [
 65   '   Panel' 5�   .   �� 5�   2   .R�5�   6   >  8System.Windows.Controls.Primitives.DataGridRowsPresenter  5�   7   5�   3   5�   +    5�   &     5�   *    5�   .   �� 5�   2   .R�5�   6   �� $��txtAttributeValue��6M   -   5        $  10��5�   >   $��120px��6�   $��26��6�   #! [ 6�   #��[	 5�   \   $��4q�6�   $��Center=�5�   >   ��5�   f   �� *AttributeValue+" 5�   >   �� *AttributeName+5�   �   5�   3   5�   +     5�   *    5�   .   �� 5�   2   .R�5�   6   �� $��txtDescription��6M   -   5        $  10��5�   >   $��120px��6�   $��26��6�   $" Description��6�   #��[	 6�   $��0,4,0,4q�6�   $��Center=�6�   ��6c   �� *Description+5�   �   5   3   5  +     5  *    5  .   �� 5  2   .R�5  6   �� $��	btnDelete��6=   -   5        $��11��5  N   $��11��6Z   $��3,6,6,6q�6~   #��[ 6�   $��Center=�6�   .��5  :   �� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��6@   $��10��6�   $��10��6�   6�   5	  7   5
  3   5  +     5  *    5  .   �� 5  2   .R�5  6   	� $��	rbPrimary��6B   -   5        |�.    rbstyle#��[ 5  �   $��0,4,4,4q�6#  $��Center=�65  ( d� 	IsChecked( 6e   �� *	Isprimary+) 	� 	GroupName) 6�   �� *AttributeName+" 6�   �� *AttributeId+6R  5  3    5  .   �� 5  2   "  68   .�5  6   ��  � �� .  6R    5  6   P� 5  :    �@ 
IsSelected . . �5  >   ��  � ��
 ���� 6[   �� 5  >    � ��
 �   �6[   �� 5  >    � ��$ 0q�6a   5  ;   5  3   5   +   5"  #   5$     5%     5&     5'     �� 5(     -   5        $��tabItemHistory��5(  +   $��History��6   
�������.��5)     �� -   5        $��	brHistory��5)     
�������5+     5,     5-     5.     5/     50     