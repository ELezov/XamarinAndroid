using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using System.IO;
using Android;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Support.Design.Widget;

namespace MyFirstProject
{
	public class AboutMeFragment : Android.Support.V4.App.Fragment
	{
		private Java.IO.File _file;
		private ImageView myPhoto;
		//Bitmap bitmap;
		static int CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE = 1888;
		static readonly int REQUEST_CAMERA = 0;

		View layout;

		static string[] PERMISSIONS_CAMERA = {
			Manifest.Permission.Camera,
			Manifest.Permission.WriteExternalStorage
		};

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.RetainInstance = true;
			// Create your fragment here
		}


		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
			AboutMeFragment busrouteFragment = new AboutMeFragment();
			return busrouteFragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.aboutMeFragment, null);


			myPhoto = (ImageView)root.FindViewById(Resource.Id.imageView1);
			Button button = (Button)root.FindViewById(Resource.Id.myButton);

			//createDirectory();

			_file = null;
			_file =new Java.IO.File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "avatar.png");
			if (_file.Exists())
				myPhoto.SetImageURI(Uri.FromFile(_file));

			button.Click += delegate
				{
					//SelectImage();
				ShowCamera();

				};

			return root;
		}

		public void ShowCamera()
		{
			if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.Camera) != (int)Android.Content.PM.Permission.Granted)
			{
				RequestCameraPermission();
			}
			else {
				TakePhoto();
			}
		}

		void RequestCameraPermission()
		{

			if (ActivityCompat.ShouldShowRequestPermissionRationale(Activity, Manifest.Permission.Camera))
			{
				Snackbar
					.Make(layout, Resource.String.permission_camera_rationale, Snackbar.LengthIndefinite)
					  .SetAction(Resource.String.ok, new Action<View>(delegate (View obj)
						{
							ActivityCompat.RequestPermissions(Activity, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
						}))
					  .Show(); 
			}
			else {
				ActivityCompat.RequestPermissions(Activity, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			if (requestCode == REQUEST_CAMERA)
			{
				if (grantResults.Length == 1 && grantResults[0] == Android.Content.PM.Permission.Granted)
				{
					Snackbar.Make(layout, Resource.String.permision_available_camera, Snackbar.LengthShort).Show();
					
				}
				else {
					Snackbar.Make(layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
				}
			}
			else {
				base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			}
		}


		public void TakePhoto()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			StartActivityForResult(intent, CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE);
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == 0)
				return;
			if (requestCode == CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE)
			{
				if (resultCode == -1)
				{
					Bundle extras = data.Extras;
					Bitmap bmp = (Bitmap)extras.Get("data");
					Bitmap bitmap=getPictureFromCam(bmp);
					myPhoto.SetImageBitmap(bitmap);
					SavePicture(bitmap);
				}

			}
		}


		private Bitmap getPictureFromCam(Bitmap bmp)
		{
			var stream = new MemoryStream();
			bmp.Compress(Bitmap.CompressFormat.Png, 100, stream);
			byte[] byteArray = stream.ToArray();
			// convert byte array to Bitmap
			Bitmap bitmap = BitmapFactory.DecodeByteArray(byteArray, 0,
													  byteArray.Length);
			return bitmap;
		}


		private void SavePicture(Bitmap bitmap)
		{
			Java.IO.File file = new Java.IO.File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "avatar.png");
			try
			{
				using (var os = new System.IO.FileStream(file.AbsolutePath, System.IO.FileMode.Create))
				{
					bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
				}
			}
			catch (Exception ex)
			{
				System.Console.Write(ex.Message);
			}
		}

		private void SelectImage()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
			alert.SetTitle("Add photo");
			alert.SetNeutralButton("Take Photo", (EventHandler<DialogClickEventArgs>)null);
			alert.SetPositiveButton("Cancel", (EventHandler<DialogClickEventArgs>)null);

			var dialog = alert.Create();
			dialog.Show();

			var takePhotoBtn = dialog.GetButton((int)DialogButtonType.Neutral);
			var cancelBtn = dialog.GetButton((int)DialogButtonType.Positive);

			takePhotoBtn.Click += (sender, args) =>
			{
				dialog.Dismiss();
				ShowCamera();
			};
			cancelBtn.Click += (sender, args) =>
			{
				dialog.Dismiss();
			};
		}
	}
}
