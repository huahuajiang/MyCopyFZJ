   M S B A M L   `   `   `  ����E  ABaseWindow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:BaseWindow
BaseWindow  5   	   L HHB_UserControls, Version=19.12.3.1, Culture=neutral, PublicKeyToken=nullP9clr-namespace:HB_UserControls.UC;assembly=HB_UserControlsHB_UserControls.UC 5   	   A%clr-namespace:BaseWindow.UserControlsBaseWindow.UserControls  5   	       BaseWindow.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:BaseWindow     5   	   HUC9clr-namespace:HB_UserControls.UC;assembly=HB_UserControls     5   	   5WUC%clr-namespace:BaseWindow.UserControls     5   	   -   5          � Title$  
MainWindow��5
   	   $
��1024��6   $	��768��6)    � WindowStyle$
 None=�67    � WindowStartupLocation$ CenterScreen=�6J    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�6�    � 	Resources 5         
btn_submit �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� Orientation �� Triggers P� Property P� Value �� 
TargetName 
 TxtPwd
 }� Pen >� 	Thickness >� Brush >� 
EndLineCap >� StartLineCap >� DashCap >� 	DashStyle �� Dashes �� Offset }� Location+  %System.Windows.TextDecorationLocation��%z  &	        &	 T    �� 5   
   " ��6#   .�5      ��  �@�Cursor$
 Hand��6'   66   �� 5       �@J�SnapsToDevicePixels . 6C   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $
 
Horizontal=�6%   .+�5      �� $��BtnBg��6$   $ ��UI_Images/btn_submit_1.png��61   5       5      P� 5       � �� . . �5   "   �� $ BtnBg��6^    � ��$  UI_Images/btn_submit_2.png��6;   6^   5      P� 5       � {� .  . �5   "   �� $ BtnBg��6^    � ��$  UI_Images/btn_submit_3.png��6;   6^   5      5      5      5       �� 5!   
   " ��6   .�5"      ��  � ��$ 30��6'   �� 5#       � ��
 ���� 6A   �� 5$       � ��$ 20(�6)   66   �� 5%       � ��$ Courier New)�6+   6A   �� 5&       �@��TextDecorations 5'      |� 5(      }� 5)       5*      >� 5+   "   $ 10Q�6&   
 �   �$ Round=�6C   $ Round=�6V   $ Round=�6k    5,   &   �� 5-   *   $ 0.0,1.2R�64   $	 0.6Q�6E   5/   #    51       52   "   Strikethrough=�6H   54      55      57      59      5:      .��5;      �� -   5         ��Name$ 
MainCanvas��5;      .+�5<   
   �� -   5         ��Name$ 	img_Tilte��5<       �� Left$ 40��6#   
 �� Top$ 11��64   6F   �� 5=   
   -   5         ��Name$ 	lbl_Title��5=      $��黑体)�6#   $��30(�63   $��Black"�6A   ���#d1ecfd$ 83��6i   $ 10��6z   $	��621��6�   6�   �� 5>   
   -   5        $ lbl_version��5>      $�� ��6%   $��15(�60   $��Center=�6>   $��Center=�6b    �� Bottom$ 0��6�   $��34��6�   $
��1024��6�    � 5?   
   -   5          �Name$ 	mainFrame��5?         � NavigationUIVisibility$  Hidden=�6#   ! J� SnapsToDevicePixels! . 6�   *  $HB_UserControls.UC.UC_Keyboard_Admin  5@   
   -   5        "  Name$" uc_keyboard��5@      $ 0��64   $ 0��6D   6U   ,   &BaseWindow.UserControls.MarqueeControl  5B   
   -   5        #  Name$# scrollingTextControl��5B      $ 0��6;   $ 0��6K   $��34��6]   $	��300��6i   $��15(�6u   $   ShowType$$ Right=�6�   %   Speed$
% 0.02Q�6�   $	 724��6�   6�   �� 5C   
   -	   5        $ CanSreenPrt��5C      .+�5D      �� -
   5        $ can_bg��5D      ��5E      �� 5F      & �� ImageSource$& UI_Images/bg_pb.png��6!   6E   5H      �� 5J      -   5        $ can_pwd��5J      $	��480��6&   .+�5K      �� $ 0��6   $ 0��6'   ' J� OpacityMask' 5L      �� 5M      $& UI_Images/word_pb.png��6%   5O      �� 5P      -   5        $ label��5P      $-��'请输入管理员密码，退出屏保��6'   $ 0��6?   ���#585858$��35(�6c   $ 0��6q   6�   �� 5Q      -   5        ( ��Name$( txtB_pwd��5Q      #��[ 6,   $��72��6L   $ 0��6X   $ 56��6h   $	��352��6x   $��35(�6�   $��Center=�6�   �� 5R      -   5        ) ��Name$) 
btn_submit��5R      #��[  6-   $	 353��6Q   $ 56��6c   6�   5S      5T      5U      5V      