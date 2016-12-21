using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Hubo.Droid
{
    public class ResizeBugWorkaround
    {
        public static void assistActivity(Activity activity)
        {
            new ResizeBugWorkaround(activity);
        }

        private View mChildOfContent;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;

        private ResizeBugWorkaround(Activity activity)
        {
            FrameLayout content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);
            mChildOfContent = content.GetChildAt(0);
            ViewTreeObserver vto = mChildOfContent.ViewTreeObserver;
            vto.GlobalLayout += (object sender, EventArgs e) =>
            {
                possiblyResizeChildAtContent();
            };
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void possiblyResizeChildAtContent()
        {
            int usableHeightNow = computeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;

                frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;

                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            Rect r = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(r);
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                return (r.Bottom - r.Top);
            }

            return r.Bottom;
        }
    }
}