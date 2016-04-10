﻿//
//  Copyright (c) 2011, Neil Alexander T.
//  All rights reserved.
// 
//  Redistribution and use in source and binary forms, with
//  or without modification, are permitted provided that the following
//  conditions are met:
// 
//  - Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
//  LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//  SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//

using System;

namespace ClashRoyaleProxy
{

	public class curve25519xsalsa20poly1305
	{
		public const int crypto_secretbox_PUBLICKEYBYTES = 32;
		public const int crypto_secretbox_SECRETKEYBYTES = 32;
		public const int crypto_secretbox_BEFORENMBYTES = 32;
		public const int crypto_secretbox_NONCEBYTES = 24;
		public const int crypto_secretbox_ZEROBYTES = 32;
		public const int crypto_secretbox_BOXZEROBYTES = 16;

		public static int crypto_box_getpublickey(byte[] pk, byte[] sk)
		{
			return curve25519.crypto_scalarmult_base(pk, sk);
		}

		public static int crypto_box_keypair(byte[] pk, byte[] sk)
		{
            new Random().NextBytes(sk);
			return curve25519.crypto_scalarmult_base(pk, sk);
		}

		public static int crypto_box_afternm(byte[] c, byte[] m, long mlen, byte[] n, byte[] k)
		{
			return xsalsa20poly1305.crypto_secretbox(c, m, mlen, n, k);
		}

		public static int crypto_box_beforenm(byte[] k, byte[] pk, byte[] sk)
		{
			byte[] s = new byte[32];
			byte[] sp = s, sigmap = xsalsa20.sigma;

			curve25519.crypto_scalarmult(sp, sk, pk);
			return hsalsa20.crypto_core(k, null, sp, sigmap);
		}

		public static int crypto_box(byte[] c, byte[] m, long mlen, byte[] n, byte[] pk, byte[] sk)
		{
			byte[] k = new byte[crypto_secretbox_BEFORENMBYTES];
			byte[] kp = k;

			crypto_box_beforenm(kp, pk, sk);
			return crypto_box_afternm(c, m, mlen, n, kp);
		}

		public static int crypto_box_open(byte[] m, byte[] c, long clen, byte[] n, byte[] pk, byte[] sk)
		{
			byte[] k = new byte[crypto_secretbox_BEFORENMBYTES];
			byte[] kp = k;

			crypto_box_beforenm(kp, pk, sk);
			return crypto_box_open_afternm(m, c, clen, n, kp);
		}

		public static int crypto_box_open_afternm(byte[] m, byte[] c, long clen, byte[] n, byte[] k)
		{
			return xsalsa20poly1305.crypto_secretbox_open(m, c, clen, n, k);
		}

		public static int crypto_box_afternm(byte[] c, byte[] m, byte[] n, byte[] k)
		{
			byte[] cp = c, mp = m, np = n, kp = k;
			return crypto_box_afternm(cp, mp, (long)m.Length, np, kp);
		}

		public static int crypto_box_open_afternm(byte[] m, byte[] c, byte[] n, byte[] k)
		{
			byte[] cp = c, mp = m, np = n, kp = k;
			return crypto_box_open_afternm(mp, cp, (long) c.Length, np, kp);
		}

		public static int crypto_box(byte[] c, byte[] m, byte[] n, byte[] pk, byte[] sk)
		{
			byte[] cp = c, mp = m, np = n, pkp = pk, skp = sk;
			return crypto_box(cp, mp, (long) m.Length, np, pkp, skp);
		}

		public static int crypto_box_open(byte[] m, byte[] c, byte[] n, byte[] pk, byte[] sk)
		{
			byte[] cp = c, mp = m, np = n, pkp = pk, skp = sk;
			return crypto_box_open(mp, cp, (long) c.Length, np, pkp, skp);
		}
	}

}