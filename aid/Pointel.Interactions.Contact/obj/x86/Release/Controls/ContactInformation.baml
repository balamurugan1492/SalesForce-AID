   M S B A M L   `   `   `  ���� W  SPointel.Interactions.Contact, Version=5.0.3.8, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Pointel.Interactions.Contact.Converters'Pointel.Interactions.Contact.Converters  >    8Pointel.Interactions.Contact.Controls.ContactInformation   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6my-http://schemas.microsoft.com/wpf/2008/toolkit  E	Converter5clr-namespace:Pointel.Interactions.Contact.Converters  -   $��Calibri)�$��12(�  � 	Resources   �� MergedDictionaries �� SourceG   APointel.Interactions.Contact.Converters.NameToVisibilityConverter   Key  MediaTemplateVoice  MediaTemplateEmail  MediaTemplateChat��  �� $T N/Pointel.Interactions.Contact;component/Resource/StylesResourceDictionary.xaml?���   NameToVisibilityConverter  %r  &	       &	 r     &	 �     �� 
VoiceMedia��.R��� $��16��$��16��$C��=/Agent.Interaction.Desktop;component/Images/Contact/Voice.png���� 
EmailMedia��.R��� $��16��$��16��$C��=/Agent.Interaction.Desktop;component/Images/Contact/Email.png���� 	ChatMedia��.R��� $��16��$��16��$B��</Agent.Interaction.Desktop;component/Images/Contact/Chat.png��.���� $
��Auto�����#D6D7D6$��1q�._�� .+��� -   $��stk_Options��$��5q� K� ZIndex$
 1000�� �� Orientation$ 
Horizontal=�$��Right=�$	��Top=�$��25��.+��� -   $��btnSave��$��22��$��22��$��0,0,5,0q�  NormalImageButton#��[ $	��Top=�$��Stretch=�.���� -   $��imgSave��$	��0.5q�����  �� 
TargetType" ��.���  �� Property � �� �� Value$E ?/Agent.Interaction.Desktop;component/Images/Contact/Save-01.png�� �� Triggers �� 	 �� Binding	 �� 
 �� ElementName$
 btnSave�� �� Path$ 	IsEnabled� �� Value	 False.a���  � ��$K E/Agent.Interaction.Desktop;component/Images/Contact/Save-Disabled.png�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  � RowDefinitions �� $
��Auto ��� $��* �.+��� -   $��SaveHeading��$��Save Contact��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��SaveContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$#��Save the contact information.���� -   $��btnReset��$��22��$��22��#��[ $	��Top=�$��Stretch=�.���� -   $��imgReset��$	��0.5q����� " ��.���  � ��$F @/Agent.Interaction.Desktop;component/Images/Contact/Reset-01.png�� �� 	 �� $
 btnReset��$ 	IsEnabled�	 False.a���  � ��$L F/Agent.Interaction.Desktop;component/Images/Contact/Reset-Disabled.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -	   $��ResetHeading��$��Reset Contact��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -
   $��ResetContent��$$��Reset the contact information.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $��
tbCallBack��$��2q� �� SelectedIndex$ 0��
�������.@��� -   $��tabitemInformation��
�������$��Information�� �� 
IsSelected . .��� -   $��grdMain��
������  �� -    �Name$ rowDockError��$��30 ��� $��* �.+�� $
��Auto��$
��Auto��$��0��
������� � ColumnDefinitions �� $��30 ��� -   $ grdErrorColumn��$��* ��� $��30 �.+��� $;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��0��$��5q�$��20��$��20���� -   $��lblError��$��1��$
��Wrap=�$��Center=�$��5q�;9Can't save this contact , Enter the fields before saving.�� -   $��btnCloseError��$��Right=�$��2��$��20��$��20��#��[ $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Close Error��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��Close the error messages.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $��scv_Information��$��2,4,2,2q�$��1��$��Stretch=�$
��Auto=�$
��Auto=�   P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%M  '��       7  7  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� -   $��
stkPnlInfo��$ Vertical=�.+��� $��8q�$��General��$
��Auto�����#ADAAAD$	��0.5q�.��)  #Microsoft.Windows.Controls.DataGrid  -   $��dgGeneralInfo��   GridLinesVisibility$
 None=�
������ $��0q�
������    AutoGenerateColumns .     CanUserAddRows .     HeadersVisibility$
 None=�|�.     Columns 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	 120    
IsReadOnly .    CellTemplate �� .R��� $ 
Horizontal=�$	��100��.+��� |�.  $��4,4,0,4q����� *AttributeName+�� $�� *��$��0,2,4,0q�
���  ��|�.  $��Calibri)�$��14(�y��� *AttributeName+  NameToVisibilityConverter �� 	Converter# [    	CellStyle �� -  'Microsoft.Windows.Controls.DataGridCell"  .���  � �� .   P�  �@ 
IsSelected . . ���  � ��
 ���� ��  � ��
 �   ���  � ��$ 0q�  $	 120  �� .R��� -     �� 	UndoLimit$  10��$��120px��$��26��  DefaultCopyContextmenu! � ContextMenu#! [  	 MyWaterMarkStyle#��[	 $��0,4,0,4q�$��Center=����� *AttributeValue+
" � Tag" �� *AttributeName+   �� .R��� $��btnDeleteSingleAtt��-   $��11��$��11��$��3,6,0,6q�#��[ $��Center=�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��  -   $��dgGroupData��$
 None=�
������ 
������  .   .  $
 None=�# +� FocusedElement# �� $
 txtAttributeValue��   
 	GroupData$ �� 
TargetType  NormalButton��%�  &	
       7  7  �� " ��.���  � �� �� "$ ��.R��� $��10q����� $ 
Horizontal=�.+��� ���� *Name+�� $�� *��
���  ��$��Calibri)�$��14(�y��� *Name+8   .���� $ Vertical=�.+��� �� $��btnAdd��-   $��23��$
��Left=�$	��180��$��4q�$��5,0,5,0q�8�� .���� $ 
Horizontal=�.+��� $
��Add ��
��������� 
����������� *Name+% �� 
GroupStyle% (  "System.Windows.Controls.GroupStyle  &   ContainerStyle#& [
 '   Panel' �� .R�>  8System.Windows.Controls.Primitives.DataGridRowsPresenter      �� .R��� $��txtAttributeValue��-   $  10��$��120px��$��26��#! [ #��[	 $��4q�$��Center=����� *AttributeValue+" �� *AttributeName+   �� .R��� $��txtDescription��-   $  10��$��120px��$��26��$" Description��#��[	 $��0,4,0,4q�$��Center=����� *Description+   �� .R��� $��	btnDelete��-   $��11��$��11��$��3,6,6,6q�#��[ $��Center=�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��$��10��$��10��   �� .R�	� $��	rbPrimary��-   |�.    rbstyle#��[ $��0,4,4,4q�$��Center=�( d� 	IsChecked( �� *	Isprimary+) 	� 	GroupName) �� *AttributeName+" �� *AttributeId+ �� "  .���  � �� .   P�  �@ 
IsSelected . . ���  � ��
 ���� ��  � ��
 �   ���  � ��$ 0q��� -   $��tabItemHistory��$��History��
�������.���� -   $��	brHistory��
�������