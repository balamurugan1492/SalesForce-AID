   M S B A M L   `   `   `  ���� '  #Pointel.Windows.Views.Common.EditorB    <Pointel.Windows.Views.Common.Editor.Controls.HyperLinkPicker   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     
�������  � 	Resources     NormalButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius J� SnapsToDevicePixels �� Triggers P� Property	 P� Value
 �� 
TargetName :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration g� AutoReverse  controltempTextbox �� Padding ��Name��%�  &	        '�� �    &	 [    �� " ��.���  � �� �� " ��.R��� $��Border��$ 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. # z #��z�� P�  �@J�IsMouseOver	 . . ��� $
 Border�� � �� �#007edf:� $ Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$ 1X�$
 0.25X�$ 0:0:0.25D� . P�  � ��	 . . ��� $
 Border�� � �� �#0061ac�� " ��.���  � ��
 �   ���  � ��$ Calibri)���  � ��$ 12(���  � �� �� " ��.R��� #��z��._��� $��5q�#��z��#��z��# z ��.  �� " ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q�.��� $
��Auto��.+�]� 
������ .+��� $	��Top=�._���  �� Orientation$ 
Horizontal=�$
��Left=�$��4q�.+��� $K��E/Pointel.Windows.Views.Common.Editor;component/Images/New/Add URL.png��$��2,0,0,0q�$��32��$��32���� $��Insert HyperLink��$��2,0,0,0q�$��14(�$
��Bold"��� $��Bottom=�$ 8q�._�]� .+��� $ 
Horizontal=�$��Right=�$��Right=�.+��� -    ��Name$ btnOK��$	��8,0q�$��25��$��OK��$��56��#��[  -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 2��   TabNavigation$ Local=��� -   $ 	btnCancel��$��Cancel��#��[  $��56��$ 3��$ Local=��� $ 
Horizontal=�$
��Left=�$
��Left=�.+��� -   $��lblError��
���  ���� $ 8q�._��  � ColumnDefinitions �� $
��Auto ��� $	��250 � � RowDefinitions �� $��33 ��� $��33 �.+��� $	��URL��$��0��$��2q��� $
��Text��$��1��$��2q��� -   $��txtURL��$��1��$��0�� �� 	UndoLimit$ 10��$��2q�#��[ �� -   $��txtText��$��1��$��1��$ 10��$��2q�#��[ 