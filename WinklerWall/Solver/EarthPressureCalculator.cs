using System;

namespace WinklerWall.Solver
{
    public static class EarthPressureCalculator
    {
        public static double CalculateK0(
            double phiDegrees)
        {
            double phi =
                phiDegrees *
                Math.PI /
                180.0;

            return
                1.0 -
                Math.Sin(phi);
        }

        public static double CalculateKa(
            double phiDegrees)
        {
            double phi =
                phiDegrees *
                Math.PI /
                180.0;

            double sinPhi =
                Math.Sin(phi);

            return
                (1.0 - sinPhi) /
                (1.0 + sinPhi);
        }

        public static double CalculateKp(
            double phiDegrees)
        {
            double phi =
                phiDegrees *
                Math.PI /
                180.0;

            double sinPhi =
                Math.Sin(phi);

            return
                (1.0 + sinPhi) /
                (1.0 - sinPhi);
        }

        public static double VerticalEffectiveStress(
            double gamma,
            double z)
        {
            return gamma * z;
        }

        public static double InitialPressure(
            double k0,
            double gamma,
            double z)
        {
            return
                k0 *
                VerticalEffectiveStress(
                    gamma,
                    z);
        }

        public static double ActivePressure(
            double ka,
            double gamma,
            double z)
        {
            return
                ka *
                VerticalEffectiveStress(
                    gamma,
                    z);
        }

        public static double PassivePressure(
            double kp,
            double gamma,
            double z)
        {
            return
                kp *
                VerticalEffectiveStress(
                    gamma,
                    z);
        }
    }
}