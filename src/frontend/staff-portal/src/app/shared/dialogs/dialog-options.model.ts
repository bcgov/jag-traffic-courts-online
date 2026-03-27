export interface DialogOptions {
  icon?: string;
  iconColor?: string; // Custom color for the icon (e.g., '#FFC107' for yellow)
  imageSrc?: string; // Alternative to an icon

  titleKey?: string;
  messageKey?: string;
  actionTextKey?: string;
  cancelTextKey?: string;

  // title?: string;
  message?: string;
  actionType?: 'primary' | 'accent' | 'warn' | 'green';
  // actionText?: string;
  actionHide?: boolean;
  actionLink?: {
    href: string;
    target: '_self' | '_blank';
    text: string;
  };
  // cancelText?: string;
  cancelHide?: boolean;
  component?: any;
  data?: any[];
}
