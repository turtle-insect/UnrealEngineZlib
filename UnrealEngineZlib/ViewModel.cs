using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealEngineZlib
{
	internal class ViewModel
	{
		public CommandAction DeCompCommand { get; set; }
		public CommandAction CompCommand { get; set; }

		public ViewModel()
		{
			DeCompCommand = new CommandAction(DeComp);
			CompCommand = new CommandAction(Comp);
		}

		private void DeComp(Object? obj)
		{
			var openDlg = new OpenFileDialog();
			if (openDlg.ShowDialog() == false) return;

			Byte[] buffer = System.IO.File.ReadAllBytes(openDlg.FileName);
			Byte[] output = new Byte[0];
			for (int index = 0; index < buffer.Length;)
			{
				int size = BitConverter.ToInt32(buffer, index + 0x10);
				Byte[] comp = new Byte[size];
				Array.Copy(buffer, index + 0x30, comp, 0, comp.Length);
				try
				{
					Byte[] tmp = Ionic.Zlib.ZlibStream.UncompressBuffer(comp);
					Array.Resize(ref output, output.Length + tmp.Length);
					Array.Copy(tmp, 0, output, output.Length - tmp.Length, tmp.Length);
					if (BitConverter.ToInt32(buffer, index + 0x18) != 0x20000) break;
					index += size + 0x30;
				}
				catch
				{
					return;
				}
			}

			var saveDlg = new SaveFileDialog();
			if (saveDlg.ShowDialog() == false) return;
			System.IO.File.WriteAllBytes(saveDlg.FileName, output);
		}

		private void Comp(Object? obj)
		{
			var openDlg = new OpenFileDialog();
			if (openDlg.ShowDialog() == false) return;

			Byte[] buffer = System.IO.File.ReadAllBytes(openDlg.FileName);
			Byte[] output = new Byte[0];
			for (int index = 0; index < buffer.Length; index += 0x20000)
			{
				int size = 0x20000;
				if (index + size > buffer.Length) size = buffer.Length - index;

				Byte[] decomp = new Byte[size];
				Array.Copy(buffer, index, decomp, 0, decomp.Length);
				Byte[] tmp = Ionic.Zlib.ZlibStream.CompressBuffer(decomp);
				int length = output.Length;
				Array.Resize(ref output, length + tmp.Length + 0x30);
				Array.Copy(BitConverter.GetBytes(0x9E2A83C1), 0, output, length, 4);
				Array.Copy(BitConverter.GetBytes(0x20000), 0, output, length + 8, 4);
				Array.Copy(BitConverter.GetBytes(tmp.Length), 0, output, length + 0x10, 4);
				Array.Copy(BitConverter.GetBytes(size), 0, output, length + 0x18, 4);
				Array.Copy(BitConverter.GetBytes(tmp.Length), 0, output, length + 0x20, 4);
				Array.Copy(BitConverter.GetBytes(size), 0, output, length + 0x28, 4);
				Array.Copy(tmp, 0, output, length + 0x30, tmp.Length);
			}

			var saveDlg = new SaveFileDialog();
			if (saveDlg.ShowDialog() == false) return;
			System.IO.File.WriteAllBytes(saveDlg.FileName, output);
		}
	}
}
