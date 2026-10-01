import type { ComponentType } from 'react';
import { ClearSky } from './ClearSky';
import { Cloudy } from './Cloudy';
import { Fog } from './Fog';
import { Rainy } from './Rainy';
import { Snow } from './Snow';
import { Thunderstorm } from './Thunderstorm';

interface AnimationProps {
  isDay: boolean;
}

const animationsByCode: Record<number, ComponentType<AnimationProps>> = {
  0: ClearSky,
  2: Cloudy,
  45: Fog,
  61: Rainy,
  71: Snow,
  95: Thunderstorm,
};

export function getAnimationForCode(code: number, _isDay: boolean): ComponentType<AnimationProps> {
  return animationsByCode[code] ?? ClearSky;
}
