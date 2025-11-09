// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.
// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.
// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.
// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.
// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MExpress.Mex
{
	public class ResImageSet_Toggle
	{
		public static ImageSet_Toggle Visibility = new ImageSet_Toggle()
		{
			ImageTrue = (BitmapImage) new ImageColorConverter().Convert(ResImage.visibility_off, null, ResColor.icon_primary, null),
			ImageFalse = (BitmapImage) new ImageColorConverter().Convert(ResImage.visibility, null, ResColor.icon_primary, null),
		};

		public static ImageSet_Toggle DropdownOpen = new ImageSet_Toggle()
		{
			ImageTrue = (BitmapImage) new ImageColorConverter().Convert(ResImage.expand_less, null, ResColor.icon_primary, null),
			ImageFalse = (BitmapImage) new ImageColorConverter().Convert(ResImage.expand_more, null, ResColor.icon_primary, null),
		};

	}
}
