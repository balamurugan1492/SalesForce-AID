   M S B A M L   `   `   `  ����   Pointel.Interactions.Contact?    9Pointel.Interactions.Contact.Forms.ContactDirectoryWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  ContactDirectoryWindow1��5       � Title$ Contact Directory��5   	    � AllowsTransparency .   � WindowStyle$
 None=�6>    � 
ResizeMode$ NoResize=�6Q   
������  � SizeToContent$ WidthAndHeight=�5   	    � Icon$D >/Agent.Interaction.Desktop;component/Images/Email/Email-01.png��6(   $	��600��6p   $	��595��6|    � ShowInTaskbar .  $��Calibri)�5   @    � 	Resources 5      	 �� MergedDictionaries
 �� Source�� 5	   
   	 5
      �� 5      $B
 </Pointel.Interaction.Workbin;component/Resource/Generic.xaml?�5      %    5      .��5      �� -   5        $��
MainBorder��5      $��0��6   
����������#0070C5$��1,1,1,1q�6U   ._�5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      .+�5      �� -   5        $��	pTitleBar��5      $��22��6&   ._�5      �  � ColumnDefinitions 5      �� 5      $��25 �6+   �� 5      $��* �6+   �� 5       $��70 �6+   .+�5"      �� -   5        $��agentStateBorder��5"      $��0��65   $��5,0,0,0q�6E   
�������$��60��6i   ._�5#      ��  �� Orientation$ 
Horizontal=�6%   .+�5$      �� -   5        $��imgEmail��5$   $   $��12��65   $��Center=�6A   $C��=/Pointel.Interactions.Email;component/Images/E-Mail/Email.png��6\   $��1q�6�   5%      5&      �� 5(      -   5        $��txtAgentName��5(   p   ���#007EDF$��Calibri)�65   $��Contact Search��6J   $��1��6`   
�������$��15(�6�   5*      �� 5+      -   5        $��txtCustomerName��5+      $��14(�63   $��Calibri)�6A   $��0��6W   ���#007edf
�������$
��Left=�6�   $��	33,-1,0,1q�6�   5,      �� 5-      -   5        $��btnClose��5-   k   $��25��6   $��2��6(   $��Right=�68   $��Right=�6O      CloseButton#��[  6{   6�   5.      5/      ]� 50      -	   5        $��dockContactDirectoryPanel��50      $��1��69   $	��573��6F   $	��598��6V   $��Stretch=�6e   $��Stretch=�6�   51      52      54      