   M S B A M L   `   `   `  ����   Pointel.Interactions.Email@    :Pointel.Interactions.Email.UserControls.InboundUserControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -     � 	Resources   �� MergedDictionaries �� Source��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��2q�
�������._��  � RowDefinitions �� $
��Auto ��� $��* �.+�� -   $��gdMain��$��2q�$��0�� �� �� �� �� �� �� .+��� -   $��txtInboundSubject�� �� 	UndoLimit$ 10��$��0�� �� 
IsReadOnly . $
��Bold"�$��5,0,10,0q� �� TextWrapping$
 Wrap=�   DefaultCopyContextmenu � ContextMenu# [  .������  �� 
TargetType" ��	 �� Triggers	 �� 
 �� Binding
 �� *IsKeyboardFocused+ �� RelativeSource �� *Self+ �� Value	 false.a���  �� Property � �� �� Value ��  �� 
TargetType" ��.R��� #��z��$��CharacterEllipsis=�# [  ����  �� ElementName$ gdMain�� �� Path$ Width� �� Triggers P�  P� Property �@J�	IsFocused P� Value . . ���  � �� �#ADAAAD��  � ��$ 1q���  � ��
 �����P�  �@J�	IsFocused .  . ���  � ��
 ���� ��  � ��$ 1q���  � ��
 �����.��]� -   $��dockFrom��$��1��.+��� $��Center=�$��40��.��From :�� -   $��txtInboundFrom��$ 10��
������ 
������ $��0q� . $
��Auto��$
 Wrap=�$��Center=�$��Stretch=�# [  ��Z� �� -   $��lblInboundDateTime��$ 10��
������ 
������ $��0q� . $��Right=�$
��Auto��$��5,0,5,0q�# [  ��Z� ]� -   $��dockTo��$��2��.+��� $��Center=�$��35��.��To :�� -	   $��txtInboundTo��$ 10��$
��Auto��$
 Wrap=�$��Center=�
������ 
������ $��0q� . # [  ��Z� .��]� -
   $��dockCc��$��3��.+��� $��Center=�$��35��.��Cc :�� -   $��txtInboundCc��$ 10��$
��Auto��$
 Wrap=�$��Center=�
������ 
������ $��0q� . # [  ��Z� ]� -   $��dockBcc��$��4��.+��� $��Center=�$��35��.��Bcc :�� -   $��txtInboundBcc��$ 10��$
��Auto��$
 Wrap=�$��Center=�
������ 
������ $��0q� . # [  ��Z� �� -   $��attacScroll��$��5��$
��Auto��$��60��$��Stretch=���. $��Hidden=�$
��Auto=�.��� -   $��wrapInboundAttachments��$
��Auto=�� -   $��dockInboundContent��$��1��