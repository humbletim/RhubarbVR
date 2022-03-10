﻿using System;

using RhuEngine.DataStructure;
using RhuEngine.WorldObjects.ECS;

using OpusDotNet;

using StereoKit;
using System.Collections.Generic;

namespace RhuEngine.WorldObjects
{
	public class OpusStream : AudioStream
	{
		[OnChanged(nameof(LoadOpus))]
		[Default(Application.VoIP)]
		public Sync<Application> typeOfStream;

		[OnChanged(nameof(LoadOpus))]
		[Default(Bandwidth.MediumBand)]
		public Sync<Bandwidth> MaxBandwidth;

		[OnChanged(nameof(LoadOpus))]
		[Default(true)]
		public Sync<bool> DTX;

		[Default(64000)]
		public Sync<int> BitRate;
		
		private OpusDecoder _decoder;
		private OpusEncoder _encoder;


		public override bool IsRunning => (_encoder is not null) && (_decoder is not null);

		private void LoadOpus() {
			if (_encoder is not null) {
				_encoder.Dispose();
				_encoder = null;
			}
			if (_decoder is not null) {
				_decoder.Dispose();
				_decoder = null;
			}
			try {
				_encoder = new OpusEncoder(typeOfStream.Value, 48000, 1) {
					VBR = true,
					DTX = DTX,
					MaxBandwidth = MaxBandwidth
				};
				_decoder = new OpusDecoder(48000, 1);
			}
			catch (Exception ex) {
				Log.Err($"Exception when loading Opus {ex}");
			}
		}

		public override void OnLoaded() {
			base.OnLoaded();
			LoadOpus();
		}

		public override byte[] SendAudioSamples(float[] audio) {
			var outpack = new byte[BitRate.Value/8];
			var amount = _encoder.Encode(audio, SampleCount, outpack, outpack.Length);
			Array.Resize(ref outpack, amount);
			return outpack;
		}

		public override float[] ProssesAudioSamples(byte[] data) {
			var audio = new float[SampleCount];
			_decoder.Decode(data, data?.Length??0, audio, audio.Length);
			return audio;
		}
	}
}
